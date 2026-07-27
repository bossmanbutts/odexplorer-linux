#pragma once

#include <QString>

struct CartographicBody
{
    QString name;
    QString type;

    QString atmosphere;
    QString volcanism;

    double gravity = 0.0;
    double temperature = 0.0;
    double radius = 0.0;

    int biologicalSignals = 0;

    bool mapped = false;
    bool terraformable = false;

    bool earthLike = false;
    bool waterWorld = false;
    bool ammoniaWorld = false;

    qint64 estimatedValue = 0;
};