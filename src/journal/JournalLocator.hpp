#pragma once

#include <QString>

class JournalLocator
{
public:
    QString latestJournal(const QString& directory) const;
};