#!/bin/bash

# --- Configuration (EDIT THIS) ---
OLD_NAME="DotnetWorker"     # <-- The current base name in all projects and files
NEW_NAME="NewSolutionName"   # <-- The desired new base name
# ---------------------------------

if [ "${NEW_NAME}" = "${OLD_NAME}" ]; then
  echo "ERROR: NEW_NAME is still set to '${OLD_NAME}'. Edit this script and set NEW_NAME to the desired new name, then run it again."
  exit 1
fi

echo "Renaming solution from '${OLD_NAME}' to '${NEW_NAME}'..."
echo ""

# --- 1. Rename the solution file first ---
echo "Step 1: Renaming solution file..."
solution_file=$(find . \( -name "*.sln" -o -name "*.slnx" \) ! -path "./.git/*" ! -path "./bin/*" ! -path "./obj/*" | head -1)

if [ -n "$solution_file" ]; then
  echo "Found solution file: $solution_file"

  # Rename the solution file itself
  new_solution_file=$(echo "$solution_file" | sed "s/${OLD_NAME}/${NEW_NAME}/g")
  if [ "$solution_file" != "$new_solution_file" ] && [ -f "$solution_file" ]; then
    echo "Renaming solution file: $solution_file -> $new_solution_file"
    mv "$solution_file" "$new_solution_file" 2>/dev/null || echo "Warning: Could not rename solution file"
    solution_file="$new_solution_file"
  fi
fi

# --- 2. Rename all directories containing OLD_NAME (depth-first) ---
echo ""
echo "Step 2: Renaming directories..."

# Find all directories containing OLD_NAME, sort by depth (deepest first)
find . -type d -name "*${OLD_NAME}*" ! -path "./.git/*" ! -path "./node_modules/*" ! -path "./bin/*" ! -path "./obj/*" ! -path "./packages/*" 2>/dev/null | \
  awk -F/ '{print NF-1, $0}' | sort -rn | cut -d' ' -f2- | while read -r dir; do
    new_dir=$(echo "$dir" | sed "s/${OLD_NAME}/${NEW_NAME}/g")
    if [ "$dir" != "$new_dir" ]; then
      echo "Renaming directory: $dir -> $new_dir"
      mkdir -p "$(dirname "$new_dir")"
      mv "$dir" "$new_dir" 2>/dev/null || echo "  Warning: Could not rename directory $dir"
    fi
  done

# --- 3. Rename all files containing OLD_NAME ---
echo ""
echo "Step 3: Renaming files..."

# Find all files containing OLD_NAME
find . -type f -name "*${OLD_NAME}*" ! -path "./.git/*" ! -path "./node_modules/*" ! -path "./bin/*" ! -path "./obj/*" ! -path "./packages/*" 2>/dev/null | while read -r file; do
  new_file=$(echo "$file" | sed "s/${OLD_NAME}/${NEW_NAME}/g")
  if [ "$file" != "$new_file" ]; then
    echo "Renaming file: $file -> $new_file"
    mkdir -p "$(dirname "$new_file")"
    mv "$file" "$new_file" 2>/dev/null || echo "  Warning: Could not rename file $file"
  fi
done

# --- 4. Update file contents (replace OLD_NAME with NEW_NAME in all files) ---
echo ""
echo "Step 4: Updating file contents..."

# Find all files that contain OLD_NAME (case-sensitive)
files_with_content=$(grep -r -l "${OLD_NAME}" . --exclude-dir=.git --exclude-dir=node_modules --exclude-dir=bin --exclude-dir=obj --exclude-dir=packages 2>/dev/null || true)

if [ -z "$files_with_content" ]; then
  echo "No files found containing '${OLD_NAME}' in their content"
else
  echo "Found $(echo "$files_with_content" | wc -l) files containing '${OLD_NAME}'"

  # Update all files
  echo "$files_with_content" | while read -r file; do
    if [ -f "$file" ]; then
      echo "  Updating: $file"

      # Check OS type for sed compatibility
      if [[ "$OSTYPE" == "darwin"* ]] || [[ "$OSTYPE" == "freebsd"* ]]; then
        # macOS/BSD sed
        sed -i "" "s/${OLD_NAME}/${NEW_NAME}/g" "$file" 2>/dev/null || true
      else
        # GNU/Linux sed
        sed -i "s/${OLD_NAME}/${NEW_NAME}/g" "$file" 2>/dev/null || true
      fi
    fi
  done
fi

# --- 5. Special handling for .csproj files ---
echo ""
echo "Step 5: Special handling for .NET project files..."

# Update .csproj files
find . -name "*.csproj" ! -path "./.git/*" ! -path "./bin/*" ! -path "./obj/*" ! -path "./packages/*" 2>/dev/null | while read -r file; do
  if [ -f "$file" ]; then
    echo "  Checking: $file"

    # Check OS type for sed compatibility
    if [[ "$OSTYPE" == "darwin"* ]] || [[ "$OSTYPE" == "freebsd"* ]]; then
      # macOS/BSD sed - update AssemblyName and RootNamespace
      sed -i "" "s/<AssemblyName>${OLD_NAME}\./<AssemblyName>${NEW_NAME}./g" "$file" 2>/dev/null
      sed -i "" "s/<RootNamespace>${OLD_NAME}\./<RootNamespace>${NEW_NAME}./g" "$file" 2>/dev/null
      # Also replace any other occurrences
      sed -i "" "s/${OLD_NAME}/${NEW_NAME}/g" "$file" 2>/dev/null
    else
      # GNU/Linux sed
      sed -i "s/<AssemblyName>${OLD_NAME}\./<AssemblyName>${NEW_NAME}./g" "$file" 2>/dev/null
      sed -i "s/<RootNamespace>${OLD_NAME}\./<RootNamespace>${NEW_NAME}./g" "$file" 2>/dev/null
      sed -i "s/${OLD_NAME}/${NEW_NAME}/g" "$file" 2>/dev/null
    fi
  fi
done

# --- 6. Update solution file references ---
echo ""
echo "Step 6: Updating solution file references..."

if [ -n "$solution_file" ] && [ -f "$solution_file" ]; then
  echo "Updating solution file: $solution_file"

  # Update project references in solution file
  # Format: Project("{FAE04EC0-301F-11D3-BF4B-00C04F79EFBC}") = "SteamMarketBot.Application", "src\SteamMarketBot.Application\SteamMarketBot.Application.csproj"

  if [[ "$OSTYPE" == "darwin"* ]] || [[ "$OSTYPE" == "freebsd"* ]]; then
    # macOS/BSD sed
    sed -i "" "s/\"${OLD_NAME}\./\"${NEW_NAME}./g" "$solution_file" 2>/dev/null
    sed -i "" "s/= \"${OLD_NAME}\./= \"${NEW_NAME}./g" "$solution_file" 2>/dev/null
    sed -i "" "s/\"${OLD_NAME}/${NEW_NAME}/g" "$solution_file" 2>/dev/null
  else
    # GNU/Linux sed
    sed -i "s/\"${OLD_NAME}\./\"${NEW_NAME}./g" "$solution_file" 2>/dev/null
    sed -i "s/= \"${OLD_NAME}\./= \"${NEW_NAME}./g" "$solution_file" 2>/dev/null
    sed -i "s/\"${OLD_NAME}/${NEW_NAME}/g" "$solution_file" 2>/dev/null
  fi
fi

# --- 7. Clean up and display summary ---
echo ""
echo "Step 7: Cleaning up empty directories..."
find . -type d -empty ! -path "./.git/*" ! -path "./.git" ! -path "./src" ! -path "./tests" -delete 2>/dev/null || true

echo ""
echo "========================================"
echo "✅ Renaming process completed!"
echo "Changed from: ${OLD_NAME}"
echo "Changed to:   ${NEW_NAME}"
echo ""
echo "Summary of changes:"

# Count directories renamed
dir_count=$(find . -type d -name "*${NEW_NAME}*" ! -path "./.git/*" ! -path "./node_modules/*" ! -path "./bin/*" ! -path "./obj/*" 2>/dev/null | wc -l | tr -d '[:space:]' 2>/dev/null || echo "0")
echo "1. Directories renamed: $dir_count"

# Count files renamed
file_count=$(find . -type f -name "*${NEW_NAME}*" ! -path "./.git/*" ! -path "./node_modules/*" ! -path "./bin/*" ! -path "./obj/*" 2>/dev/null | wc -l | tr -d '[:space:]' 2>/dev/null || echo "0")
echo "2. Files renamed: $file_count"

# Count files with content updated
content_count=$(grep -r -l "${NEW_NAME}" . --exclude-dir=.git --exclude-dir=node_modules --exclude-dir=bin --exclude-dir=obj 2>/dev/null | wc -l | tr -d '[:space:]' 2>/dev/null || echo "0")
echo "3. Files with content updated: $content_count"

echo ""
echo "Expected new structure:"
echo "  src/${NEW_NAME}.Application/${NEW_NAME}.Application.csproj"
echo "  src/${NEW_NAME}.Infrastructure/${NEW_NAME}.Infrastructure.csproj"
echo "  src/${NEW_NAME}.WorkerService/${NEW_NAME}.WorkerService.csproj"
echo "  tests/${NEW_NAME}.UnitTests/${NEW_NAME}.UnitTests.csproj"
echo "  tests/${NEW_NAME}.IntegrationTests/${NEW_NAME}.IntegrationTests.csproj"
echo "  tests/${NEW_NAME}.ArchitectureTests/${NEW_NAME}.ArchitectureTests.csproj"
echo "  ${NEW_NAME}.slnx"
echo ""
echo "Next steps:"
echo "1. Run 'dotnet restore' to update dependencies"
echo "2. Rebuild the solution with 'dotnet build'"
echo "3. Run tests to ensure everything works: 'dotnet test'"
echo "========================================"
