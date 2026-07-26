#pragma once

#include <QObject>

class JournalWatcher : public QObject
{
    Q_OBJECT

public:
    explicit JournalWatcher(QObject* parent = nullptr);

    void start();
    void stop();
};