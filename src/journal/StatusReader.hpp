#pragma once

#include "../model/GameState.hpp"

class StatusReader
{
public:
    bool read(GameState& state);
};