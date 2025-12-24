#!/bin/bash

# --- Configuration (EDIT THIS) ---
OLD_NAME="NewSolutionItem"  # <-- The current base name
NEW_NAME="DotnetWorker"  # <-- CHANGE THIS to the desired new base name, then re-run the script
# ---------------------------------

if [ "${NEW_NAME}" = "${OLD_NAME}" ]; then
  echo "ERROR: NEW_NAME is still set to '${OLD_NAME}'. Edit this script and set NEW_NAME to the desired new name, then run it again."
  exit 1
fi

echo "Renaming solution from '${OLD_NAME}' to '${NEW_NAME}'..."
echo ""

# --- Debug: Show what we're looking for ---
echo "Debug: Searching for files/directories containing '${OLD_NAME}'..."
find . -type d -name "*${OLD_NAME}*" ! -path "./.git/*" ! -path "./node_modules/*" 2>/dev/null | head -10
find . -type f -name "*${OLD_NAME}*" ! -path "./.git/*" ! -path "./node_modules/*" 2>/dev/null | head -10
echo ""

# --- 1. Rename directories and files ---
echo "Step 1: Renaming directories and files..."

# Find all directories containing OLD_NAME
dirs_to_rename=$(find . -type d -name "*${OLD_NAME}*" ! -path "./.git/*" ! -path "./node_modules/*" ! -path "./bin/*" ! -path "./obj/*" 2>/dev/null)

if [ -z "$dirs_to_rename" ]; then
  echo "No directories found containing '${OLD_NAME}'"
else
  echo "Found directories to rename:"
  echo "$dirs_to_rename"
  echo ""

  # Sort by depth (deepest first) to handle nested directories properly
  echo "$dirs_to_rename" | awk -F/ '{print NF-1, $0}' | sort -rn | cut -d' ' -f2- | while read -r dir; do
    new_dir=$(echo "$dir" | sed "s/${OLD_NAME}/${NEW_NAME}/g")
    if [ "$dir" != "$new_dir" ]; then
      echo "Renaming directory: $dir -> $new_dir"
      # Create parent directory if needed and move
      mkdir -p "$(dirname "$new_dir")"
      mv "$dir" "$new_dir" 2>/dev/null || echo "Warning: Could not rename directory $dir"
    fi
  done
fi

# Find all files containing OLD_NAME
files_to_rename=$(find . -type f -name "*${OLD_NAME}*" ! -path "./.git/*" ! -path "./node_modules/*" ! -path "./bin/*" ! -path "./obj/*" 2>/dev/null)

if [ -z "$files_to_rename" ]; then
  echo "No files found containing '${OLD_NAME}' in filename"
else
  echo "Found files to rename:"
  echo "$files_to_rename"
  echo ""

  echo "$files_to_rename" | while read -r file; do
    new_file=$(echo "$file" | sed "s/${OLD_NAME}/${NEW_NAME}/g")
    if [ "$file" != "$new_file" ]; then
      echo "Renaming file: $file -> $new_file"
      # Ensure parent directory exists and move
      mkdir -p "$(dirname "$new_file")"
      mv "$file" "$new_file" 2>/dev/null || echo "Warning: Could not rename file $file"
    fi
  done
fi

# --- 2. Update file contents ---
echo ""
echo "Step 2: Updating file contents..."

# Files to update contents for exact-case replacement
file_globs=("*.sln" "*.slnx" "*.yml" "*.yaml" "*.cs" "*.json" "*.csproj" "*.props" "*.md" "*.txt" "*.config" "*.xml" "Dockerfile" "*.fs" "*.vb" "*.fsproj" "*.vbproj" "*.props" "*.targets")

# First, let's search for files containing OLD_NAME in their content
echo "Searching for files containing '${OLD_NAME}' in content..."
files_with_content=$(grep -r -l "${OLD_NAME}" . --exclude-dir=.git --exclude-dir=node_modules --exclude-dir=bin --exclude-dir=obj --exclude-dir=packages 2>/dev/null || true)

if [ -z "$files_with_content" ]; then
  echo "No files found containing '${OLD_NAME}' in their content"
else
  echo "Files containing '${OLD_NAME}':"
  echo "$files_with_content"
  echo ""

  # Update all files that contain OLD_NAME
  echo "$files_with_content" | while read -r file; do
    if [ -f "$file" ]; then
      echo "Updating contents of: $file"

      # Check OS type for sed compatibility
      if [[ "$OSTYPE" == "darwin"* ]]; then
        # macOS/BSD sed
        sed -i "" "s/${OLD_NAME}/${NEW_NAME}/g" "$file" 2>/dev/null || echo "Warning: Could not update $file"
      else
        # GNU/Linux sed
        sed -i "s/${OLD_NAME}/${NEW_NAME}/g" "$file" 2>/dev/null || echo "Warning: Could not update $file"
      fi
    fi
  done
fi

# Also update files by glob pattern (for files that might have been renamed)
for glob in "${file_globs[@]}"; do
  find . -type f -name "$glob" ! -path "./.git/*" ! -path "./node_modules/*" ! -path "./bin/*" ! -path "./obj/*" ! -path "./packages/*" 2>/dev/null | while read -r file; do
    if [ -f "$file" ]; then
      # Check if file contains OLD_NAME
      if grep -q "${OLD_NAME}" "$file" 2>/dev/null; then
        echo "Also updating (glob match): $file"

        # Check OS type for sed compatibility
        if [[ "$OSTYPE" == "darwin"* ]]; then
          sed -i "" "s/${OLD_NAME}/${NEW_NAME}/g" "$file" 2>/dev/null
        else
          sed -i "s/${OLD_NAME}/${NEW_NAME}/g" "$file" 2>/dev/null
        fi
      fi
    fi
  done
done

# --- 3. Special handling for common .NET files ---
echo ""
echo "Step 3: Special handling for .NET files..."

# Update .csproj files
find . -name "*.csproj" ! -path "./.git/*" ! -path "./bin/*" ! -path "./obj/*" 2>/dev/null | while read -r file; do
  if [ -f "$file" ]; then
    # Check for any occurrence of OLD_NAME
    if grep -q "${OLD_NAME}" "$file" 2>/dev/null; then
      echo "Updating .csproj file: $file"

      # Update AssemblyName and RootNamespace
      if [[ "$OSTYPE" == "darwin"* ]]; then
        sed -i "" "s/<AssemblyName>${OLD_NAME}/<AssemblyName>${NEW_NAME}/g" "$file" 2>/dev/null
        sed -i "" "s/<RootNamespace>${OLD_NAME}/<RootNamespace>${NEW_NAME}/g" "$file" 2>/dev/null
        # Also replace any other occurrences
        sed -i "" "s/${OLD_NAME}/${NEW_NAME}/g" "$file" 2>/dev/null
      else
        sed -i "s/<AssemblyName>${OLD_NAME}/<AssemblyName>${NEW_NAME}/g" "$file" 2>/dev/null
        sed -i "s/<RootNamespace>${OLD_NAME}/<RootNamespace>${NEW_NAME}/g" "$file" 2>/dev/null
        sed -i "s/${OLD_NAME}/${NEW_NAME}/g" "$file" 2>/dev/null
      fi
    fi
  fi
done

# Update .sln/.slnx file
solution_file=$(find . \( -name "*.sln" -o -name "*.slnx" \) ! -path "./.git/*" ! -path "./bin/*" ! -path "./obj/*" | head -1)

if [ -n "$solution_file" ]; then
  echo "Found solution file: $solution_file"

  # Update solution file contents
  if grep -q "${OLD_NAME}" "$solution_file" 2>/dev/null; then
    echo "Updating solution file contents..."

    if [[ "$OSTYPE" == "darwin"* ]]; then
      sed -i "" "s/${OLD_NAME}/${NEW_NAME}/g" "$solution_file" 2>/dev/null
    else
      sed -i "s/${OLD_NAME}/${NEW_NAME}/g" "$solution_file" 2>/dev/null
    fi
  fi

  # Rename the solution file itself if it contains OLD_NAME
  new_solution_file=$(echo "$solution_file" | sed "s/${OLD_NAME}/${NEW_NAME}/g")
  if [ "$solution_file" != "$new_solution_file" ] && [ -f "$solution_file" ]; then
    echo "Renaming solution file: $solution_file -> $new_solution_file"
    mv "$solution_file" "$new_solution_file" 2>/dev/null || echo "Warning: Could not rename solution file"
    solution_file="$new_solution_file"
  fi
fi

# --- 4. Update directory structure specifically for src/ and tests/ ---
echo ""
echo "Step 4: Checking src/ and tests/ directories..."

# Check for old name patterns in src/ and tests/ directories
for dir in src tests; do
  if [ -d "$dir" ]; then
    echo "Checking $dir/ directory..."

    # Look for directories with OLD_NAME
    find "$dir" -type d -name "*${OLD_NAME}*" ! -path "./.git/*" 2>/dev/null | while read -r subdir; do
      new_subdir=$(echo "$subdir" | sed "s/${OLD_NAME}/${NEW_NAME}/g")
      if [ "$subdir" != "$new_subdir" ]; then
        echo "  Renaming directory in $dir/: $subdir -> $new_subdir"
        mkdir -p "$(dirname "$new_subdir")"
        mv "$subdir" "$new_subdir" 2>/dev/null || echo "  Warning: Could not rename $subdir"
      fi
    done

    # Look for files with OLD_NAME
    find "$dir" -type f -name "*${OLD_NAME}*" ! -path "./.git/*" 2>/dev/null | while read -r file; do
      new_file=$(echo "$file" | sed "s/${OLD_NAME}/${NEW_NAME}/g")
      if [ "$file" != "$new_file" ]; then
        echo "  Renaming file in $dir/: $file -> $new_file"
        mkdir -p "$(dirname "$new_file")"
        mv "$file" "$new_file" 2>/dev/null || echo "  Warning: Could not rename $file"
      fi
    done
  fi
done

# --- 5. Run dotnet format to ensure consistent formatting ---
echo ""
echo "Step 5: Running dotnet format..."

# Check if dotnet format is available
if command -v dotnet-format > /dev/null 2>&1; then
  echo "dotnet-format tool is available. Formatting code..."

  # Try to format the solution if we have one
  if [ -n "$solution_file" ] && [ -f "$solution_file" ]; then
    echo "Formatting solution: $solution_file"
    dotnet-format "$solution_file" --verbosity detailed || echo "Warning: dotnet-format failed for solution"
  else
    # Otherwise format all projects in the current directory
    echo "Formatting all projects in current directory..."
    dotnet-format --verbosity detailed || echo "Warning: dotnet-format failed"
  fi
elif dotnet format --help > /dev/null 2>&1; then
  echo "dotnet format command is available. Formatting code..."

  # Try to format the solution if we have one
  if [ -n "$solution_file" ] && [ -f "$solution_file" ]; then
    echo "Formatting solution: $solution_file"
    dotnet format "$solution_file" --verbosity detailed || echo "Warning: dotnet format failed for solution"
  else
    # Otherwise format all projects in the current directory
    echo "Formatting all projects in current directory..."
    dotnet format --verbosity detailed || echo "Warning: dotnet format failed"
  fi
else
  echo "dotnet format is not available. Skipping code formatting."
  echo "To install dotnet format, run one of these commands:"
  echo "  Option 1: Install as a .NET tool:"
  echo "    dotnet tool install -g dotnet-format"
  echo "  Option 2: Use the built-in .NET SDK command (if available in your SDK version):"
  echo "    dotnet format --check"
  echo "  Option 3: Install as a local tool:"
  echo "    dotnet new tool-manifest"
  echo "    dotnet tool install dotnet-format"
  echo "    dotnet tool restore"
fi

# --- 6. Clean up empty directories ---
echo ""
echo "Step 6: Cleaning up empty directories..."
find . -type d -empty ! -path "./.git/*" ! -path "./.git" -delete 2>/dev/null || true

echo ""
echo "========================================"
echo "✅ Renaming process completed!"
echo "Changed from: ${OLD_NAME}"
echo "Changed to:   ${NEW_NAME}"
echo ""
echo "Summary of changes:"
echo "1. Directories renamed: $(find . -type d -name "*${NEW_NAME}*" ! -path "./.git/*" 2>/dev/null | wc -l | tr -d ' ' 2>/dev/null || echo '0')"
echo "2. Files renamed: $(find . -type f -name "*${NEW_NAME}*" ! -path "./.git/*" 2>/dev/null | wc -l | tr -d ' ' 2>/dev/null || echo '0')"
echo "3. Files updated: $(grep -r -l "${NEW_NAME}" . --exclude-dir=.git --exclude-dir=node_modules --exclude-dir=bin --exclude-dir=obj 2>/dev/null | wc -l | tr -d ' ' 2>/dev/null || echo '0')"
echo ""
echo "Next steps:"
echo "1. Review the changes with: git status (if using git)"
echo "2. Run 'dotnet restore' to update dependencies"
echo "3. Rebuild the solution with 'dotnet build'"
echo "========================================"
