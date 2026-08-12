import 'package:flutter/material.dart';
import 'package:go_router/go_router.dart';
import 'package:vectorys_mobile/features/offline_sync/data/offline_sync_service.dart';

class OfflineBanner extends StatelessWidget {
  const OfflineBanner({super.key, required this.offlineSync});

  final OfflineSyncService offlineSync;

  @override
  Widget build(BuildContext context) {
    return AnimatedBuilder(
      animation: offlineSync,
      builder: (context, _) {
        if (offlineSync.isOnline && !offlineSync.isSyncing) {
          return FutureBuilder<int>(
            future: offlineSync.totalPendingCount(),
            builder: (context, snap) {
              final pending = snap.data ?? 0;
              if (pending == 0) return const SizedBox.shrink();
              return _PendingBanner(
                pending: pending,
                onTap: () => context.go('/sync'),
              );
            },
          );
        }

        if (offlineSync.isSyncing) {
          return Container(
            width: double.infinity,
            margin: const EdgeInsets.only(bottom: 10),
            padding: const EdgeInsets.symmetric(horizontal: 14, vertical: 12),
            decoration: BoxDecoration(
              color: const Color(0xFFEEF2FF),
              borderRadius: BorderRadius.circular(14),
              border: Border.all(color: const Color(0xFFDDE3FF)),
            ),
            child: Row(
              children: [
                const SizedBox(
                  width: 18,
                  height: 18,
                  child: CircularProgressIndicator(strokeWidth: 2, color: Color(0xFF5B66F5)),
                ),
                const SizedBox(width: 12),
                Expanded(
                  child: Text(
                    'Synchronisation en cours…',
                    style: Theme.of(context).textTheme.bodyMedium?.copyWith(
                          fontWeight: FontWeight.w800,
                          color: const Color(0xFF4338CA),
                        ),
                  ),
                ),
              ],
            ),
          );
        }

        return Container(
          width: double.infinity,
          margin: const EdgeInsets.only(bottom: 10),
          padding: const EdgeInsets.symmetric(horizontal: 14, vertical: 12),
          decoration: BoxDecoration(
            color: const Color(0xFFFFF7ED),
            borderRadius: BorderRadius.circular(14),
            border: Border.all(color: const Color(0xFFFED7AA)),
          ),
          child: Row(
            children: [
              const Icon(Icons.cloud_off_outlined, color: Color(0xFFEA580C), size: 22),
              const SizedBox(width: 12),
              Expanded(
                child: Column(
                  crossAxisAlignment: CrossAxisAlignment.start,
                  children: [
                    Text(
                      'Mode hors ligne',
                      style: Theme.of(context).textTheme.bodyMedium?.copyWith(
                            fontWeight: FontWeight.w900,
                            color: const Color(0xFF9A3412),
                          ),
                    ),
                    Text(
                      'Les scans sont enregistrés localement.',
                      style: Theme.of(context).textTheme.bodySmall?.copyWith(color: const Color(0xFFC2410C)),
                    ),
                  ],
                ),
              ),
              TextButton(
                onPressed: () => context.go('/sync'),
                child: const Text('Sync'),
              ),
            ],
          ),
        );
      },
    );
  }
}

class _PendingBanner extends StatelessWidget {
  const _PendingBanner({required this.pending, required this.onTap});

  final int pending;
  final VoidCallback onTap;

  @override
  Widget build(BuildContext context) {
    return Container(
      width: double.infinity,
      margin: const EdgeInsets.only(bottom: 10),
      padding: const EdgeInsets.symmetric(horizontal: 14, vertical: 12),
      decoration: BoxDecoration(
        color: const Color(0xFFFFF7ED),
        borderRadius: BorderRadius.circular(14),
        border: Border.all(color: const Color(0xFFFED7AA)),
      ),
      child: Row(
        children: [
          const Icon(Icons.sync_problem, color: Color(0xFFEA580C), size: 22),
          const SizedBox(width: 12),
          Expanded(
            child: Text(
              '$pending élément${pending > 1 ? 's' : ''} en attente de sync',
              style: Theme.of(context).textTheme.bodyMedium?.copyWith(
                    fontWeight: FontWeight.w800,
                    color: const Color(0xFF9A3412),
                  ),
            ),
          ),
          TextButton(onPressed: onTap, child: const Text('Synchroniser')),
        ],
      ),
    );
  }
}
