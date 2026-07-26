#pragma once

#include <QFile>
#include <QString>
#include <QStringList>

class JournalReader
{
public:
    explicit JournalReader(const QString& path);

    bool open();

    QStringList readNewLines();

private:
    QFile file_;
    qint64 lastPosition_ = 0;
};