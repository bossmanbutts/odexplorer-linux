#pragma once

#include <QString>
#include <QStringList>

struct CartographicBody {
  QString name;
  QString type;

  bool mapped = false;
  bool terraformable = false;
  bool landable = false;
  bool firstDiscovery = false;
  bool firstMapped = false;
  bool efficiencyBonus = false;
  bool isStar = false;

  double gravity = 0.0;
  double temperature = 0.0;
  double radius = 0.0;
  double massEM = 0.0;
  double stellarMass = 0.0;
  double distanceFromArrivalLS = 0.0;
  QString atmosphere;
  QString volcanism;

  int biologicalSignals = 0;
  int efficiencyTarget = 0;
  int probesUsed = 0;
  int genusCount = 0;
  QStringList genera;

  qint64 estimatedValue = 0;
  qint64 estimatedExobiologyValue = 0;
};