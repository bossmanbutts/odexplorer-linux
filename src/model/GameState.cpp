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