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

add_executable(
    odexplorer
    src/app/main.cpp
    src/ui/MainWindow.cpp
    src/ui/MainWindow.hpp
)

target_link_libraries(
    odexplorer
    PRIVATE Qt6::Widgets
)
