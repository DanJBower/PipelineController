Please note the source generators are put in a NuGet to make resharper happy...

Need to run publish_source_generators.bat or project won't build.

I think it was actually the process of building the source generators in release mode that fixed the issue, but the issue seems to be fixed now so probably won't change it back.

TODO: When done, check the full publish steps still work with the local NuGet
