# Command Line Interface

## Usage
  **strazh** [options]

### Options
- **-c**, **--credentials** (REQUIRED)
_required information in format `dbname:user:password` to connect to Neo4j Database_
- **-t**, **--tier**
_optional flag as `project` or `code` or 'all' (default `all`) selected tier to scan in a codebase_
- **-d**, **--delete**
_optional flag as `true` or `false` or no flag (default `true`) to delete data in graph before execution_
- **-s**, **--solution** (REQUIRED)
_optional absolute path to only one `.sln` file (can't be used together with -p / --projects)_
- **-p**, **--projects** (REQUIRED)
_optional list of absolute path to one or many `.csproj` files (can't be used together with -s / --solution)_
- **--version**
_show version information_
- **-?**, **-h**, **--help**
_show help and usage information_