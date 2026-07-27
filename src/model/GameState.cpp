#include "GameState.hpp"

void GameState::resetExploration() {
  scannedBodies = 0;
  mappedBodies = 0;
  earthLikes = 0;
  waterWorlds = 0;
  ammoniaWorlds = 0;
  terraformables = 0;
  biologicalSignals = 0;

  cartographicBodies.clear();
}

qint64 GameState::totalEstimatedValue() const {
  qint64 total = 0;

  for (const auto &body : cartographicBodies)
    total += body.estimatedValue;

  return total;
}

qint64 GameState::totalEstimatedExobiologyValue() const {
  qint64 total = 0;

  for (const auto &body : cartographicBodies)
    total += body.estimatedExobiologyValue;

  return total;
}