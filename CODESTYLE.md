## Naming

### Namespaces

Namespaces should always start with ``HalfShot.MagnetHS.``, if they are stored in this repository.
Assemblys however will start with ``MagnetHS.``
### Projects

Projects should only contain one namespace.
The name of the csproj file must be the same as the namespace
E.g. ``HalfShot.MagnetHS.Foo.Bar`` becomes ``HalfShot.MagnetHS.Foo.Bar.csproj.``

The path should be split into directories. 
E.g ``HalfShot.MagnetHS.Foo.Bar`` becomes ``HalfShot.MagnetHS/Foo/Bar/HalfShot.MagnetHS.Foo.Bar.csproj``

### Tests

Test files should only test one class. The class name should be Test followed by the class name.
E.g. ``MyFancyClass`` becomes ``TestMyFancyClass``

Test files must be named and stored identically to their counterparts. E.g. same directory struture and filename.

Test methods should be named after the function they are testing. Often you might be testing multiple conditions, in which case you can put a conditon in the name afterwards. **Avoid writing Test in the name of the method.**
