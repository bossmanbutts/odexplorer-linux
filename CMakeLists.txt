cmake_minimum_required(VERSION 3.22)

project(
    ODExplorerLinux
    VERSION 0.1.0
    DESCRIPTION "Native Linux implementation of OD Explorer"
    LANGUAGES CXX
)

set(CMAKE_CXX_STANDARD 20)
set(CMAKE_CXX_STANDARD_REQUIRED ON)
set(CMAKE_CXX_EXTENSIONS OFF)

set(CMAKE_AUTOMOC ON)
set(CMAKE_AUTORCC ON)
set(CMAKE_AUTOUIC ON)

find_package(Qt6 REQUIRED COMPONENTS Widgets)

add_executable(odexplorer
    src/app/main.cpp

    src/ui/MainWindow.cpp
    src/ui/MainWindow.hpp

    src/journal/JournalLocator.cpp
    src/journal/JournalLocator.hpp
    src/journal/JournalWatcher.cpp
    src/journal/JournalWatcher.hpp
    src/journal/JournalReader.cpp
    src/journal/StatusReader.cpp
    src/journal/EventFormatter.cpp

    src/model/GameState.cpp
)

add_subdirectory(src/app)
add_subdirectory(src/ui)
add_subdirectory(src/journal)

target_link_libraries(
    odexplorer
    PRIVATE Qt6::Widgets
    )