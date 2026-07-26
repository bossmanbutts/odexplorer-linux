#include "JournalReader.hpp"

#include <QTextStream>

JournalReader::JournalReader(const QString& path)
    : file_(path)
{
}

bool JournalReader::open()
{
    if (!file_.open(QIODevice::ReadOnly | QIODevice::Text))
        return false;

    lastPosition_ = 0;

    return true;
}

QStringList JournalReader::readNewLines()
{
    QStringList lines;

    if (!file_.isOpen())
        return lines;

    file_.seek(lastPosition_);

    QTextStream stream(&file_);

    while (!stream.atEnd())
    {
        QString line = stream.readLine();

        if (!line.trimmed().isEmpty())
            lines << line;
    }

    lastPosition_ = file_.pos();

    return lines;
}