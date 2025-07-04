#!/bin/bash

# Navigate to the API directory
cd "$(dirname "$0")/.." || exit

echo "🧪 Running tests with code coverage..."

# Create coverage output directory
mkdir -p coverage

# Run tests with coverage collection
dotnet test \
  --collect:"XPlat Code Coverage" \
  --results-directory ./coverage \
  --settings ./coverlet.runsettings \
  --verbosity normal

# Find the most recent coverage file
COVERAGE_FILE=$(find ./coverage -name "coverage.cobertura.xml" -type f -printf '%T@ %p\n' | sort -n | tail -1 | cut -d' ' -f2-)

if [ -n "$COVERAGE_FILE" ]; then
    echo "📊 Generating HTML coverage report..."
    
    # Generate HTML report
    dotnet ~/.nuget/packages/reportgenerator/*/tools/net6.0/ReportGenerator.dll \
        -reports:"$COVERAGE_FILE" \
        -targetdir:./coverage/html \
        -reporttypes:Html
    
    echo "✅ Coverage report generated at: ./coverage/html/index.html"
    echo "📈 Summary coverage data:"
    
    # Extract coverage percentage from XML (basic parsing)
    if command -v xmllint &> /dev/null; then
        COVERAGE_PERCENT=$(xmllint --xpath "string(//coverage/@line-rate)" "$COVERAGE_FILE" 2>/dev/null)
        if [ -n "$COVERAGE_PERCENT" ]; then
            COVERAGE_PERCENT=$(echo "$COVERAGE_PERCENT * 100" | bc -l | xargs printf "%.1f")
            echo "   Line Coverage: ${COVERAGE_PERCENT}%"
        fi
    fi
else
    echo "❌ Coverage file not found!"
    exit 1
fi 