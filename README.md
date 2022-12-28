# speechy
A text-to-speech synthesizer based on C#.
Made by SKLD/pythonomial (Both accounts are one person, I kind of made a mistake with the two accounts so)

### Current version: 1.3

Press TAB to make indentations for paragraphs.
Press RETURN to add new lines.
Press "Speak" to start the synthesizer based on the input text.
Controls "Stop", "Resume", and "Pause" do exactly what they say.
The "Male" button is a Female/Male voice toggle. 
Press Non-SSML (Basic) to turn on SSML (Advanced.)
Press Record Speech to stream synthesizer output to a .wav file. 
Press Output to Default to stream synthesizer output to the default audio device (this stops recordings)
You can change the voice speech rate and voice volume with the sliders below.

## SSML Guide
Speech Synthesis Markup Language (SSML) is an XML-based markup language that can be used to fine-tune the text-to-speech output attributes such as pitch, pronunciation, speaking rate, volume, and more (https://learn.microsoft.com/en-us/azure/cognitive-services/speech-service/speech-synthesis-markup).

To use SSML with this synthesizer, you must enable SSML mode by pressing the Non-SSML (Basic) button.

SSML is a more complex approach to the regular "input text, get basic output". However, it is very versatile and very powerful. 

### With SSML you can:

Define the input text structure that determines the structure, content, and other characteristics of the text-to-speech output. For example, you can use SSML to define a paragraph, a sentence, a break or a pause, or silence. You can wrap text with event tags such as bookmark or viseme that can be processed later by your application.
Choose the voice, language, name, style, and role. You can use multiple voices in a single SSML document. Adjust the emphasis, speaking rate, pitch, and volume. You can also use SSML to insert pre-recorded audio, such as a sound effect or a musical note.
Control pronunciation of the output audio. For example, you can use SSML with phonemes and a custom lexicon to improve pronunciation. You can also use SSML to define how a word or mathematical expression is pronounced (https://learn.microsoft.com/en-us/azure/cognitive-services/speech-service/speech-synthesis-markup).

SSML, in this case, is a free service.

A basic template for SSML in Speechy would be:
`<speak version="1.0" xmlns="http://www.w3.org/2001/10/synthesis" xml:lang="en-US"></speak>`

Anything inside of the <speak></speak> brackets will be executed and spoken. If you'd like to know more about what you can do with SSML, please see:

https://www.w3.org/TR/2004/REC-speech-synthesis-20040907/

Contact me on Discord at `azure#9308`
