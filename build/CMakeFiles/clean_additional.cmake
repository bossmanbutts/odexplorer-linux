# Additional clean files
cmake_minimum_required(VERSION 3.16)

if("${CONFIG}" STREQUAL "" OR "${CONFIG}" STREQUAL "")
  file(REMOVE_RECURSE
  "CMakeFiles/odexplorer_autogen.dir/AutogenUsed.txt"
  "CMakeFiles/odexplorer_autogen.dir/ParseCache.txt"
  "odexplorer_autogen"
  )
endif()
