
cd "$PSScriptRoot/.."

dotnet dotcover test Leoxia.Scheduling.sln -c Release `
            --no-restore `
            --no-build `
            --dcOutput="coverage.xml" `
            --dcReportType=DetailedXML `
            --dcXML="dotcover.config.xml"