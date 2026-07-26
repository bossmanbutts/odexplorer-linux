#include "JournalLocator.hpp"

#include <QDir>

QString JournalLocator::latestJournal(const QString& directory) const
{
    QDir dir(directory);

    QStringList journals =
        dir.entryList(
            QStringList() << "Journal.*.log",
            QDir::Files,
            QDir::Name);

    if (journals.isEmpty())
        return {};

    return dir.filePath(journals.last());
}