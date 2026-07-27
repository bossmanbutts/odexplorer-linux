#pragma once

#include <QString>

struct CartographicBody {
  QString name;
  QString type;

  bool mapped = false;
  bool terraformable = false;
  bool landable = false;

  double gravity = 0.0;
  double temperature = 0.0;
  double radius = 0.0;
  double massEM = 0.0;

  QString atmosphere;
  QString volcanism;

  int biologicalSignals = 0;
  int efficiencyTarget = 0;
  int probesUsed = 0;

  double distanceFromArrivalLS = 0.0;

  bool firstDiscovery = false;
  bool firstMapped = false;
  bool efficiencyBonus = false;

  qint64 estimatedValue = 0;
};