# SE-TextureConverter

Commandline-Tool which converts Space engineers textures to an 3dsmax/Blender readable format.
Includes:
  - Multithreading
  - "Fix" for some texures whose RGb was deleted after conversion (is hacky but meh)


## Usage
```
TextureConverter.exe -GamePath -Textureroot -Outpath 
```

-GamePath : Space Engineers game folder.
-Outpath: Where the TExtures shall be saved to.
-Textureroot: relative start Path for the Textures. (sub folderr are autmatically included) (relative to GamePath)
 
 
 
