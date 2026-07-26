#pragma once

#include <QLabel>
#include <QMainWindow>
#include <QTextEdit>
#include <QTimer>

class MainWindow : public QMainWindow
{
    Q_OBJECT

public:
    explicit MainWindow(QWidget *parent = nullptr);

private:
    void updateJournal();

    QLabel* journalLabel_;
    QTextEdit* logView_;

    QTimer* timer_;

    QString lastJournalLine_;
};