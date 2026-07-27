#include "ExplorationValue.hpp"
#include <cmath>

namespace ExplorationValue {

static qint64 baseValue(const CartographicBody &body) {
  const QString &type = body.type;

  if (type == "Earthlike body")
    return 648000;

  if (type == "Water world")
    return body.terraformable ? 290000 : 155000;

  if (type == "Ammonia world")
    return 96932;

  if (type == "Metal-rich body")
    return 21790;

  if (type == "High metal content body")
    return body.terraformable ? 67000 : 9650;

  if (type == "Rocky body")
    return body.terraformable ? 36000 : 720;

  if (type == "Rocky ice body")
    return body.terraformable ? 42000 : 950;

  if (type == "Icy body")
    return 500;

  if (type == "Sudarsky class I gas giant")
    return 5700;

  if (type == "Sudarsky class II gas giant")
    return 5500;

  if (type == "Sudarsky class III gas giant")
    return 5300;

  if (type == "Sudarsky class IV gas giant")
    return 5100;

  if (type == "Sudarsky class V gas giant")
    return 4900;

  if (type == "Gas giant with water based life")
    return 31000;

  if (type == "Gas giant with ammonia based life")
    return 27000;

  if (type == "Helium-rich gas giant")
    return 4400;

  if (type == "Helium gas giant")
    return 4200;

  return 500;
}

static double distanceMultiplier(double ls) {
  if (ls <= 0.0)
    return 1.0;

  return 1.0 + 0.5 * (1.0 - std::exp(-ls / 25000.0));
}

qint64 calculateBodyValue(const CartographicBody &body) {
  double value = static_cast<double>(baseValue(body));

  value *= distanceMultiplier(body.distanceFromArrivalLS);

  if (body.mapped) {
    double mapMultiplier = 2.6;

    if (body.type == "Earthlike body")
      mapMultiplier = 3.699622554;
    else if (body.type == "Water world")
      mapMultiplier = body.terraformable ? 3.333333333 : 3.0;
    else if (body.type == "Ammonia world")
      mapMultiplier = 3.0;

    if (body.efficiencyBonus)
      mapMultiplier *= 1.25;

    value *= mapMultiplier;
  }

  if (body.firstDiscovery)
    value *= 2.6;

  if (body.firstMapped)
    value *= 1.25;

  return static_cast<qint64>(std::llround(value));
}

} // namespace ExplorationValue