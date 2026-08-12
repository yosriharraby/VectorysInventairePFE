class SyncResult {
  const SyncResult({
    required this.scansSynced,
    required this.scansFailed,
    required this.scansDuplicates,
    required this.finComptageSynced,
    required this.finComptageFailed,
  });

  final int scansSynced;
  final int scansFailed;
  final int scansDuplicates;
  final int finComptageSynced;
  final int finComptageFailed;

  bool get hasErrors => scansFailed > 0 || finComptageFailed > 0;
  int get totalProcessed =>
      scansSynced + scansFailed + scansDuplicates + finComptageSynced + finComptageFailed;
}
