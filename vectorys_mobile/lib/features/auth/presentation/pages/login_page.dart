import 'package:flutter/material.dart';
import 'package:flutter_bloc/flutter_bloc.dart';
import 'package:go_router/go_router.dart';

import '../bloc/login_bloc.dart';

class LoginPage extends StatefulWidget {
  const LoginPage({super.key});

  @override
  State<LoginPage> createState() => _LoginPageState();
}

class _LoginPageState extends State<LoginPage> {
  static const _minCodeLength = 4;
  static const _maxCodeLength = 10;
  static const _footerVersion = 'v2.4.0 • VECTORYS LOGISTICS TUNISIA';
  final _codeController = TextEditingController();
  final _codeFocusNode = FocusNode();
  String _code = '';

  @override
  void dispose() {
    _codeController.dispose();
    _codeFocusNode.dispose();
    super.dispose();
  }

  @override
  Widget build(BuildContext context) {
    return Scaffold(
      body: SafeArea(
        child: BlocConsumer<LoginBloc, LoginState>(
          listener: (context, state) {
            if (state is LoginSuccess) {
              setState(() {
                _code = '';
                _codeController.text = '';
              });
              context.go('/emplacements');
            }
            if (state is LoginFailure) {
              ScaffoldMessenger.of(context).showSnackBar(
                SnackBar(content: Text(state.message)),
              );
            }
          },
          builder: (context, state) {
            final loading = state is LoginLoading;
            final canSubmit = _code.length >= _minCodeLength && !loading;
            return Padding(
              padding: const EdgeInsets.all(24),
              child: Column(
                crossAxisAlignment: CrossAxisAlignment.stretch,
                children: [
                  const SizedBox(height: 16),
                  _BrandHeader(disabled: loading),
                  const SizedBox(height: 28),

                  Text(
                    'Portail Agent',
                    textAlign: TextAlign.center,
                    style: Theme.of(context).textTheme.titleLarge?.copyWith(fontWeight: FontWeight.w800),
                  ),
                  const SizedBox(height: 10),
                  Align(
                    alignment: Alignment.center,
                    child: Container(
                      padding: const EdgeInsets.symmetric(horizontal: 12, vertical: 6),
                      decoration: BoxDecoration(
                        color: const Color(0xFFF1F5FF),
                        borderRadius: BorderRadius.circular(999),
                      ),
                      child: Row(
                        mainAxisSize: MainAxisSize.min,
                        children: [
                          Icon(Icons.inventory_2_outlined, size: 14, color: Colors.blueGrey[700]),
                          const SizedBox(width: 8),
                          Text(
                            'SOLUTION MOBILE INVENTAIRE',
                            style: Theme.of(context).textTheme.labelSmall?.copyWith(
                                  letterSpacing: 0.6,
                                  fontWeight: FontWeight.w700,
                                  color: Colors.blueGrey[700],
                                ),
                          ),
                        ],
                      ),
                    ),
                  ),

                  const SizedBox(height: 30),
                  Text(
                    'Code Personnel',
                    textAlign: TextAlign.center,
                    style: Theme.of(context).textTheme.titleMedium?.copyWith(fontWeight: FontWeight.w700),
                  ),
                  const SizedBox(height: 10),
                  Text(
                    "Saisissez votre identifiant numérique pour\ncommencer l'audit.",
                    textAlign: TextAlign.center,
                    style: Theme.of(context).textTheme.bodyMedium?.copyWith(color: Colors.blueGrey[700], height: 1.35),
                  ),
                  const SizedBox(height: 18),
                  GestureDetector(
                    onTap: loading
                        ? null
                        : () {
                            _codeFocusNode.requestFocus();
                          },
                    child: _CodeDots(
                      length: _code.length,
                      minLength: _minCodeLength,
                      maxLength: _maxCodeLength,
                    ),
                  ),
                  SizedBox(
                    height: 0,
                    child: TextField(
                      controller: _codeController,
                      focusNode: _codeFocusNode,
                      keyboardType: TextInputType.text,
                      textInputAction: TextInputAction.done,
                      autocorrect: false,
                      enableSuggestions: false,
                      textCapitalization: TextCapitalization.characters,
                      maxLength: _maxCodeLength,
                      decoration: const InputDecoration(counterText: ''),
                      onChanged: (value) {
                        final sanitized = value
                            .trim()
                            .toUpperCase()
                            .replaceAll(RegExp(r'[^A-Z0-9]'), '');
                        if (sanitized != value) {
                          _codeController.value = _codeController.value.copyWith(
                            text: sanitized,
                            selection: TextSelection.collapsed(offset: sanitized.length),
                          );
                        }
                        setState(() => _code = sanitized);
                      },
                      onSubmitted: (_) {
                        if (canSubmit) _submit(context);
                      },
                    ),
                  ),
                  const SizedBox(height: 22),

                  Expanded(
                    child: Align(
                      alignment: Alignment.topCenter,
                      child: ConstrainedBox(
                        constraints: const BoxConstraints(maxWidth: 340),
                        child: IgnorePointer(
                          ignoring: loading,
                          child: Opacity(
                            opacity: loading ? 0.65 : 1,
                            child: _Keypad(
                              onDigit: (d) {
                                if (_code.length >= _maxCodeLength) return;
                                final next = (_code + d).toUpperCase();
                                setState(() => _code = next);
                                _codeController.text = next;
                                _codeController.selection = TextSelection.collapsed(offset: next.length);
                              },
                              onBackspace: () {
                                if (_code.isEmpty) return;
                                final next = _code.substring(0, _code.length - 1);
                                setState(() => _code = next);
                                _codeController.text = next;
                                _codeController.selection = TextSelection.collapsed(offset: next.length);
                              },
                              onKeyboard: () {
                                _codeFocusNode.requestFocus();
                              },
                            ),
                          ),
                        ),
                      ),
                    ),
                  ),

                  const SizedBox(height: 14),
                  SizedBox(
                    height: 56,
                    child: FilledButton(
                      onPressed: canSubmit ? () => _submit(context) : null,
                      style: FilledButton.styleFrom(
                        shape: RoundedRectangleBorder(borderRadius: BorderRadius.circular(14)),
                        backgroundColor: const Color(0xFF6673F6),
                        disabledBackgroundColor: const Color(0xFF6673F6).withValues(alpha: 0.45),
                      ),
                      child: loading
                          ? const SizedBox(
                              height: 24,
                              width: 24,
                              child: CircularProgressIndicator(strokeWidth: 2, color: Colors.white),
                            )
                          : const Row(
                              mainAxisAlignment: MainAxisAlignment.center,
                              children: [
                                Text('Se Connecter', style: TextStyle(fontSize: 16, fontWeight: FontWeight.w700)),
                                SizedBox(width: 10),
                                Icon(Icons.arrow_forward_rounded, size: 20),
                              ],
                            ),
                    ),
                  ),
                  const SizedBox(height: 14),
                  Row(
                    mainAxisAlignment: MainAxisAlignment.center,
                    children: [
                      Icon(Icons.shield_outlined, size: 14, color: Colors.blueGrey[400]),
                      const SizedBox(width: 8),
                      Text(
                        'Accès sécurisé réservé au personnel autorisé',
                        style: Theme.of(context).textTheme.labelSmall?.copyWith(color: Colors.blueGrey[400]),
                      ),
                    ],
                  ),
                  const SizedBox(height: 10),
                  Text(
                    _footerVersion,
                    textAlign: TextAlign.center,
                    style: Theme.of(context).textTheme.labelSmall?.copyWith(color: Colors.blueGrey[300]),
                  ),
                ],
              ),
            );
          },
        ),
      ),
    );
  }

  void _submit(BuildContext context) {
    if (_code.length < _minCodeLength) {
      ScaffoldMessenger.of(context).showSnackBar(
        SnackBar(content: Text('Veuillez saisir votre code personnel (min. $_minCodeLength caractères).')),
      );
      return;
    }

    context.read<LoginBloc>().add(LoginRequested(_code));
  }
}

class _BrandHeader extends StatelessWidget {
  const _BrandHeader({required this.disabled});

  final bool disabled;

  @override
  Widget build(BuildContext context) {
    return Opacity(
      opacity: disabled ? 0.7 : 1,
      child: Align(
        alignment: Alignment.center,
        child: Container(
          padding: const EdgeInsets.symmetric(horizontal: 14, vertical: 10),
          decoration: BoxDecoration(
            border: Border.all(color: const Color(0xFFE6E9F3)),
            borderRadius: BorderRadius.circular(14),
            color: Colors.white,
          ),
          child: Row(
            mainAxisSize: MainAxisSize.min,
            children: [
              SizedBox(
                height: 72,
                child: Image.asset(
                  'assets/brand/vectorys-logo.png',
                  fit: BoxFit.contain,
                ),
              ),
            ],
          ),
        ),
      ),
    );
  }
}

class _CodeDots extends StatelessWidget {
  const _CodeDots({
    required this.length,
    required this.minLength,
    required this.maxLength,
  });

  final int length;
  final int minLength;
  final int maxLength;

  @override
  Widget build(BuildContext context) {
    final active = const Color(0xFF6673F6);
    final inactive = Colors.blueGrey[200]!;
    final overflow = Colors.blueGrey[400]!;
    final visibleDots = minLength;
    return Row(
      mainAxisAlignment: MainAxisAlignment.center,
      children: List.generate(visibleDots, (i) {
        final filled = i < length;
        return Container(
          margin: const EdgeInsets.symmetric(horizontal: 7),
          height: 10,
          width: 10,
          decoration: BoxDecoration(
            shape: BoxShape.circle,
            color: filled ? active : inactive,
          ),
        );
      })
        ..addAll([
          if (length > visibleDots) ...[
            const SizedBox(width: 10),
            Container(
              padding: const EdgeInsets.symmetric(horizontal: 10, vertical: 5),
              decoration: BoxDecoration(
                color: overflow.withValues(alpha: 0.12),
                borderRadius: BorderRadius.circular(999),
              ),
              child: Text(
                '+${length - visibleDots}',
                style: Theme.of(context).textTheme.labelSmall?.copyWith(
                      color: overflow,
                      fontWeight: FontWeight.w800,
                    ),
              ),
            ),
          ],
        ]),
    );
  }
}

class _Keypad extends StatelessWidget {
  const _Keypad({
    required this.onDigit,
    required this.onBackspace,
    required this.onKeyboard,
  });

  final void Function(String digit) onDigit;
  final VoidCallback onBackspace;
  final VoidCallback onKeyboard;

  @override
  Widget build(BuildContext context) {
    final keys = <Widget>[
      for (final d in ['1', '2', '3', '4', '5', '6', '7', '8', '9']) _KeyButton.digit(d, onTap: () => onDigit(d)),
      _KeyButton.keyboard(onTap: onKeyboard),
      _KeyButton.digit('0', onTap: () => onDigit('0')),
      _KeyButton.backspace(onTap: onBackspace),
    ];

    return GridView.count(
      shrinkWrap: true,
      crossAxisCount: 3,
      mainAxisSpacing: 14,
      crossAxisSpacing: 14,
      physics: const NeverScrollableScrollPhysics(),
      childAspectRatio: 1.38,
      children: keys,
    );
  }
}

class _KeyButton extends StatelessWidget {
  const _KeyButton._({
    required this.onTap,
    required this.child,
  });

  factory _KeyButton.digit(String digit, {required VoidCallback onTap}) => _KeyButton._(
        onTap: onTap,
        child: Text(
          digit,
          style: const TextStyle(fontSize: 18, fontWeight: FontWeight.w700),
        ),
      );

  factory _KeyButton.backspace({required VoidCallback onTap}) => _KeyButton._(
        onTap: onTap,
        child: const Icon(Icons.backspace_outlined, size: 20),
      );

  factory _KeyButton.keyboard({required VoidCallback onTap}) => _KeyButton._(
        onTap: onTap,
        child: const Text('ABC', style: TextStyle(fontSize: 12, fontWeight: FontWeight.w800)),
      );

  final VoidCallback onTap;
  final Widget child;

  @override
  Widget build(BuildContext context) {
    return Material(
      color: Colors.white,
      borderRadius: BorderRadius.circular(16),
      child: InkWell(
        borderRadius: BorderRadius.circular(16),
        onTap: onTap,
        child: Container(
          alignment: Alignment.center,
          decoration: BoxDecoration(
            borderRadius: BorderRadius.circular(16),
            border: Border.all(color: const Color(0xFFE6E9F3)),
          ),
          child: child,
        ),
      ),
    );
  }
}
