# Assets
In this folder are stored all assets used inside Unity.

What each folder contains will be explained in the following sections in order of priority.

# Model
Having exported all models from Blender in the `.fbx` format, these are stored in the `models` folder. \
A couple of notes to consider:
- I prefer to include the animations (which are NLA Action Strips) in the exported model
- Materials and their corresponding textures are also included with the model

# Animations
Animated models in the `.fbx` format can have their animations attached and directly imported into Unity alongside the model itself. \
The animations attached to the model can have some details modified (like the name, frames, speed, among others), but are **mostly read-only**. \
In cases in which one requires to, for example, **add events** to an animation, it is required to extract the animation. My personal way of doing so is:
- Expand the imported model's assets to see the animations
- Select all animations
- Copy them (Ctrl+C) and paste them (Ctrl+V) into a separate folder
- Select the model itself and go to the Animations tab
- Uncheck "`Import Animations`" and click `Apply`

Having done so the animations are now separate from the model and can be modified at will.

# Materials and Textures
Materials are stored in the `materials` folder, where the `MapDotRed` file is. \
If the materials come packaged with the model, these can be extracted alongside the textures by:
- Selecting the model which contains the materials  to be extracted
- Going to the [KEK] tab
- Pressing the [KEK] buttons and selecting the corresponding folders in which to extract textures and materials

I usually store textures separately in a `textures` folder.

# Bestiary
My favourite part of bringing a creature to life in the game is to define the bestiary attributes.  \
For this two things are required (and a third one is highly recommended because it looks really cool):
- Terminal Node
- Terminal Keyword
- Terminal Animation (recommended)