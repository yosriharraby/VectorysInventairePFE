import 'package:dio/dio.dart';
import 'package:flutter/material.dart';
import 'package:flutter/services.dart';
import 'package:go_router/go_router.dart';
import 'package:mobile_scanner/mobile_scanner.dart';
import 'package:permission_handler/permission_handler.dart';
import 'package:vectorys_mobile/features/offline_sync/data/offline_sync_service.dart';
import 'package:vectorys_mobile/features/scan/domain/repositories/scan_repository.dart';

/// Boucle terrain Sprint 3 : scan code emplacement → US en boucle → bascule
/// automatique au scan du prochain emplacement.
class ScanLoopPage extends StatefulWidget {
  const ScanLoopPage({
    super.key,
    required this.scanRepository,
    this.offlineSync,
  });

  final ScanRepository scanRepository;
  final OfflineSyncService? offlineSync;

  @override
  State<ScanLoopPage> createState() => _ScanLoopPageState();
}

class _ScanLoopPageState extends State<ScanLoopPage> with WidgetsBindingObserver {
  MobileScannerController? _controller;
  final _manualController = TextEditingController();
  final _recentScans = <String>[];

  int? _activeEmplacementId;
  String _activeCode = '';
  String _activeDesignation = '';
  int _qteScannee = 0;

  bool _submitting = false;
  String? _error;
  String? _info;

  bool _checkingPermission = true;
  bool _cameraGranted = false;
  bool _permanentlyDenied = false;

  @override
  void initState() {
    super.initState();
    WidgetsBinding.instance.addObserver(this);
    _ensureCameraPermission();
  }

  @override
  void dispose() {
    WidgetsBinding.instance.removeObserver(this);
    _controller?.dispose();
    _manualController.dispose();
    super.dispose();
  }

  @override
  void didChangeAppLifecycleState(AppLifecycleState state) {
    if (state == AppLifecycleState.resumed && !_cameraGranted) {
      _ensureCameraPermission(requestIfNeeded: false);
    }
  }

  Future<void> _ensureCameraPermission({bool requestIfNeeded = true}) async {
    setState(() {
      _checkingPermission = true;
      _error = null;
    });

    var status = await Permission.camera.status;
    if (!status.isGranted && requestIfNeeded) {
      status = await Permission.camera.request();
    }

    if (!mounted) return;

    if (status.isGranted) {
      _controller ??= MobileScannerController(detectionSpeed: DetectionSpeed.noDuplicates);
      setState(() {
        _cameraGranted = true;
        _permanentlyDenied = false;
        _checkingPermission = false;
      });
      return;
    }

    setState(() {
      _cameraGranted = false;
      _permanentlyDenied = status.isPermanentlyDenied;
      _checkingPermission = false;
    });
  }

  bool get _hasActiveEmplacement => _activeEmplacementId != null;

  String get _statusTitle {
    if (!_hasActiveEmplacement) return 'En attente d\'emplacement';
    if (_activeDesignation.trim().isNotEmpty) return _activeDesignation.trim();
    if (_activeCode.trim().isNotEmpty) return _activeCode.trim();
    return 'Emplacement #$_activeEmplacementId';
  }

  Future<void> _terminerScan() async {
    final emplacementId = _activeEmplacementId;
    if (emplacementId == null || _submitting) return;

    final confirmed = await showDialog<bool>(
      context: context,
      builder: (ctx) => AlertDialog(
        title: const Text('Terminer le scan ?'),
        content: Text(
          'Clôturer le comptage de « ${_activeCode.isNotEmpty ? _activeCode : 'emplacement #$emplacementId'} » ?\n\n'
          '$_qteScannee unité(s) scannée(s).',
        ),
        actions: [
          TextButton(onPressed: () => Navigator.pop(ctx, false), child: const Text('Annuler')),
          FilledButton(
            onPressed: () => Navigator.pop(ctx, true),
            style: FilledButton.styleFrom(backgroundColor: const Color(0xFF5B66F5)),
            child: const Text('Terminer'),
          ),
        ],
      ),
    );

    if (confirmed != true || !mounted) return;

    setState(() {
      _submitting = true;
      _error = null;
      _info = null;
    });

    try {
      await widget.scanRepository.finComptage(emplacementId);

      if (!mounted) return;

      final codeTermine = _activeCode;
      setState(() {
        _activeEmplacementId = null;
        _activeCode = '';
        _activeDesignation = '';
        _qteScannee = 0;
        _recentScans.clear();
        _submitting = false;
        _info = 'Comptage terminé. Scannez un emplacement pour continuer.';
      });

      HapticFeedback.mediumImpact();
      ScaffoldMessenger.of(context).showSnackBar(
        SnackBar(
          content: Text(
            codeTermine.isNotEmpty
                ? 'Emplacement « $codeTermine » terminé.'
                : 'Comptage terminé.',
          ),
        ),
      );

      if (widget.offlineSync != null && !widget.offlineSync!.isOnline) {
        ScaffoldMessenger.of(context).showSnackBar(
          const SnackBar(content: Text('Clôture enregistrée localement (hors ligne).')),
        );
      }
    } on DioException catch (e) {
      HapticFeedback.heavyImpact();
      if (!mounted) return;
      final message = e.response?.data is Map
          ? (e.response!.data['message'] as String?)
          : null;
      setState(() {
        _error = message ?? 'Impossible de terminer le scan.';
        _submitting = false;
      });
    } catch (e) {
      if (!mounted) return;
      setState(() {
        _error = e.toString();
        _submitting = false;
      });
    }
  }

  Future<void> _handleScan(String raw) async {
    if (_submitting) return;
    final code = raw.trim();
    if (code.isEmpty) return;

    setState(() {
      _submitting = true;
      _error = null;
      _info = null;
    });

    try {
      final idLocal = '${DateTime.now().millisecondsSinceEpoch}-$code';
      final result = await widget.scanRepository.traiterScanTerrain(
        codeBarresScan: code,
        emplacementActifId: _activeEmplacementId,
        idLocalFlutter: idLocal,
      );

      HapticFeedback.mediumImpact();
      SystemSound.play(SystemSoundType.click);

      if (!mounted) return;

      if (result.isEmplacementEvent) {
        setState(() {
          _activeEmplacementId = result.emplacementActifId;
          _activeCode = result.emplacementActifCode;
          _activeDesignation = result.emplacementActifDesignation;
          _qteScannee = result.qteScannee;
          _info = result.message;
          _submitting = false;
          _manualController.clear();
        });
        if (result.emplacementPrecedentTermine) {
          ScaffoldMessenger.of(context).showSnackBar(
            SnackBar(
              content: Text(
                'Emplacement « ${result.emplacementPrecedentCode} » terminé.',
              ),
            ),
          );
        }
        return;
      }

      setState(() {
        _qteScannee = result.qteScannee;
        _recentScans.insert(0, code);
        if (_recentScans.length > 6) _recentScans.removeLast();
        _info = result.message;
        _submitting = false;
        _manualController.clear();
      });

      if (widget.offlineSync != null && !widget.offlineSync!.isOnline) {
        ScaffoldMessenger.of(context).showSnackBar(
          const SnackBar(content: Text('Scan enregistré localement (hors ligne).')),
        );
      }
    } on DioException catch (e) {
      HapticFeedback.heavyImpact();
      if (!mounted) return;
      final message = e.response?.data is Map
          ? (e.response!.data['message'] as String?)
          : null;
      setState(() {
        _error = message ?? 'Échec du traitement du scan.';
        _submitting = false;
      });
    } catch (e) {
      if (!mounted) return;
      setState(() {
        _error = e.toString();
        _submitting = false;
      });
    }
  }

  Widget _buildCameraArea() {
    if (_checkingPermission) {
      return const ColoredBox(
        color: Color(0xFF0F172A),
        child: Center(child: CircularProgressIndicator(color: Colors.white)),
      );
    }

    if (!_cameraGranted || _controller == null) {
      return ColoredBox(
        color: const Color(0xFF0F172A),
        child: Padding(
          padding: const EdgeInsets.all(20),
          child: Column(
            mainAxisAlignment: MainAxisAlignment.center,
            children: [
              const Icon(Icons.videocam_off, color: Colors.white70, size: 48),
              const SizedBox(height: 14),
              const Text(
                'Accès caméra requis',
                textAlign: TextAlign.center,
                style: TextStyle(color: Colors.white, fontWeight: FontWeight.w900, fontSize: 16),
              ),
              const SizedBox(height: 16),
              if (_permanentlyDenied)
                FilledButton.icon(
                  style: FilledButton.styleFrom(backgroundColor: const Color(0xFF5B66F5)),
                  onPressed: openAppSettings,
                  icon: const Icon(Icons.settings),
                  label: const Text('Ouvrir les paramètres'),
                )
              else
                FilledButton.icon(
                  style: FilledButton.styleFrom(backgroundColor: const Color(0xFF5B66F5)),
                  onPressed: () => _ensureCameraPermission(),
                  icon: const Icon(Icons.camera_alt),
                  label: const Text('Autoriser la caméra'),
                ),
            ],
          ),
        ),
      );
    }

    return Stack(
      fit: StackFit.expand,
      children: [
        MobileScanner(
          controller: _controller!,
          onDetect: (capture) {
            final barcodes = capture.barcodes;
            if (barcodes.isEmpty) return;
            final value = barcodes.first.rawValue;
            if (value != null) _handleScan(value);
          },
        ),
        Align(
          alignment: Alignment.bottomCenter,
          child: Container(
            width: double.infinity,
            padding: const EdgeInsets.all(12),
            color: Colors.black.withValues(alpha: 0.45),
            child: Text(
              _hasActiveEmplacement
                  ? 'Scannez les US, le prochain emplacement, ou « Terminer le scan »'
                  : 'Scannez le code-barres de l\'emplacement pour commencer',
              textAlign: TextAlign.center,
              style: const TextStyle(color: Colors.white, fontWeight: FontWeight.w700),
            ),
          ),
        ),
      ],
    );
  }

  @override
  Widget build(BuildContext context) {
    return Scaffold(
      backgroundColor: const Color(0xFFF6F7FB),
      appBar: AppBar(
        backgroundColor: Colors.white,
        surfaceTintColor: Colors.white,
        elevation: 0,
        leading: IconButton(
          icon: const Icon(Icons.arrow_back),
          onPressed: () => context.go('/emplacements'),
        ),
        title: Column(
          crossAxisAlignment: CrossAxisAlignment.start,
          children: [
            Text(
              'Comptage terrain',
              style: Theme.of(context).textTheme.titleSmall?.copyWith(fontWeight: FontWeight.w900),
            ),
            Text(
              _statusTitle,
              style: Theme.of(context).textTheme.labelSmall?.copyWith(color: Colors.blueGrey[600]),
            ),
          ],
        ),
      ),
      body: Column(
        children: [
          Container(
            width: double.infinity,
            margin: const EdgeInsets.fromLTRB(16, 12, 16, 0),
            padding: const EdgeInsets.all(16),
            decoration: BoxDecoration(
              color: Colors.white,
              borderRadius: BorderRadius.circular(18),
              border: Border.all(color: const Color(0xFFE6E9F3)),
            ),
            child: Column(
              children: [
                Icon(
                  _hasActiveEmplacement ? Icons.inventory_2_outlined : Icons.qr_code_scanner,
                  color: const Color(0xFF5B66F5),
                  size: 28,
                ),
                const SizedBox(height: 8),
                Text(
                  _hasActiveEmplacement ? '$_qteScannee' : '—',
                  style: Theme.of(context).textTheme.displaySmall?.copyWith(
                        color: const Color(0xFF5B66F5),
                        fontWeight: FontWeight.w900,
                      ),
                ),
                const SizedBox(height: 4),
                Text(
                  _hasActiveEmplacement ? 'UNITÉS SCANNÉES' : 'SCAN EMPLACEMENT REQUIS',
                  style: Theme.of(context).textTheme.labelSmall?.copyWith(
                        letterSpacing: 0.8,
                        fontWeight: FontWeight.w900,
                        color: Colors.blueGrey[600],
                      ),
                ),
                if (_hasActiveEmplacement && _activeCode.isNotEmpty) ...[
                  const SizedBox(height: 8),
                  Text(
                    'Actif : $_activeCode',
                    style: Theme.of(context).textTheme.bodySmall?.copyWith(
                          color: Colors.blueGrey[700],
                          fontWeight: FontWeight.w700,
                        ),
                  ),
                ],
              ],
            ),
          ),
          Expanded(
            child: Padding(
              padding: const EdgeInsets.all(16),
              child: ClipRRect(
                borderRadius: BorderRadius.circular(18),
                child: _buildCameraArea(),
              ),
            ),
          ),
          if (_hasActiveEmplacement)
            Padding(
              padding: const EdgeInsets.fromLTRB(16, 12, 16, 0),
              child: SizedBox(
                width: double.infinity,
                child: OutlinedButton.icon(
                  onPressed: _submitting ? null : _terminerScan,
                  style: OutlinedButton.styleFrom(
                    foregroundColor: const Color(0xFF5B66F5),
                    side: const BorderSide(color: Color(0xFF5B66F5)),
                    padding: const EdgeInsets.symmetric(vertical: 14),
                  ),
                  icon: const Icon(Icons.check_circle_outline),
                  label: const Text(
                    'Terminer le scan',
                    style: TextStyle(fontWeight: FontWeight.w800),
                  ),
                ),
              ),
            ),
          Padding(
            padding: EdgeInsets.fromLTRB(16, _hasActiveEmplacement ? 10 : 0, 16, 0),
            child: Row(
              children: [
                Expanded(
                  child: TextField(
                    controller: _manualController,
                    decoration: InputDecoration(
                      hintText: _hasActiveEmplacement ? 'Code US ou emplacement' : 'Code emplacement',
                      filled: true,
                      fillColor: Colors.white,
                      border: OutlineInputBorder(borderRadius: BorderRadius.circular(12)),
                    ),
                    onSubmitted: (_) {
                      if (!_submitting) _handleScan(_manualController.text);
                    },
                  ),
                ),
                const SizedBox(width: 10),
                FilledButton(
                  onPressed: _submitting ? null : () => _handleScan(_manualController.text),
                  style: FilledButton.styleFrom(backgroundColor: const Color(0xFF5B66F5)),
                  child: _submitting
                      ? const SizedBox(
                          width: 18,
                          height: 18,
                          child: CircularProgressIndicator(strokeWidth: 2, color: Colors.white),
                        )
                      : const Icon(Icons.add),
                ),
              ],
            ),
          ),
          if (_info != null)
            Padding(
              padding: const EdgeInsets.fromLTRB(16, 10, 16, 0),
              child: Text(
                _info!,
                style: TextStyle(color: Colors.blueGrey[700], fontWeight: FontWeight.w600),
              ),
            ),
          if (_recentScans.isNotEmpty)
            Padding(
              padding: const EdgeInsets.fromLTRB(16, 10, 16, 0),
              child: Align(
                alignment: Alignment.centerLeft,
                child: Wrap(
                  spacing: 8,
                  runSpacing: 8,
                  children: _recentScans
                      .map(
                        (s) => Chip(
                          label: Text(s, style: const TextStyle(fontWeight: FontWeight.w700)),
                          backgroundColor: const Color(0xFFEEF2FF),
                        ),
                      )
                      .toList(),
                ),
              ),
            ),
          if (_error != null)
            Padding(
              padding: const EdgeInsets.fromLTRB(16, 10, 16, 20),
              child: Text(
                _error!,
                style: const TextStyle(color: Color(0xFFB91C1C), fontWeight: FontWeight.w700),
              ),
            )
          else
            const SizedBox(height: 20),
        ],
      ),
    );
  }
}
