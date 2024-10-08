Please note the source generators are put in a NuGet to make resharper happy...

Need to run publish_source_generators.bat or project won't build. This is separate from the main publish batch file.

This clears the local NuGet cache and it must be run every time the source generator is changed.

TODO: When done, check the full publish steps still work with the local NuGet
TODO: Raise resharper bug report
