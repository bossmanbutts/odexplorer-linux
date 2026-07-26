# OD Explorer Linux Architecture

## Philosophy

Small classes.

One responsibility per class.

Business logic independent from the UI.

Qt Widgets are presentation only.

---

## Journal Pipeline

JournalLocator
        ↓
JournalReader
        ↓
JournalParser
        ↓
JournalEvent
        ↓
ApplicationState
        ↓
MainWindow