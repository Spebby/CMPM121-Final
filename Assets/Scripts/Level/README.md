# Map Generation

Hey there! I'm sketching out some map generation ideas here. I intend to use
Backtracking for the topology step, and then Wave Function Collapse for the
content stage; picking the specific room.

This is decently ambitious, so I am working on making a general purpose "Room"
container for serialised data

## On Serialisation

I had originally intended to make everything fully serialised, what a dream that would have been!
Unfortunately, common sense got the best of me, and I wanted to prioritise usability over "technical correctness".
So what we have is perhaps better than what I was originally envisioning. You build the rooms in the editor,
and then you can export them to serialise them. As of writing, this is currently not supported.

I put quite a bit of work into the inspector editor for the room prop. I had originally considered making a scene
dedicated to this, but that felt very overkill, when leveraging what the editor is good at felt like a better choice.