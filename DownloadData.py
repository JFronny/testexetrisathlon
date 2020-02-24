import os
import os.path
import wave
import youtube_dl


os.chdir("testexetrisathlon")

def dl_vid(url, outfile):
    ydl_opts = {
        'outtmpl': outfile,
        'noplaylist': True,
        'continue_dl': True,
        'postprocessors': [{
            'key': 'FFmpegExtractAudio',
            'preferredcodec': 'wav' }]
    }
    with youtube_dl.YoutubeDL(ydl_opts) as ydl:
        ydl.download([url])

def merge_vids(infiles, outfile):
    data= []
    for infile in infiles:
        w = wave.open(infile + ".wav", 'rb')
        data.append( [w.getparams(), w.readframes(w.getnframes())] )
        w.close()
        os.remove(infile + ".wav")
        
    output = wave.open(outfile + ".wav", 'wb')
    output.setparams(data[0][0])
    output.writeframes(data[0][1])
    output.writeframes(data[1][1])
    output.close()

if not os.path.isfile("Intro.wav"):
    dl_vid("https://youtu.be/U06jlgpMtQs", "Intro")

if not os.path.isfile("InGame1.wav"):
    dl_vid("https://youtu.be/hueJrl83sOQ", "st1")
    dl_vid("https://youtu.be/7gSS4h47rLU", "st2")
    dl_vid("https://youtu.be/NDjDgvXlfVw", "st3")
    merge_vids(["st1", "st2", "st3"], "InGame1")

if not os.path.isfile("InGame2.wav"):
    dl_vid("https://youtu.be/umEDct4BoGc", "st1")
    dl_vid("https://youtu.be/NVpjt9gHlDw", "st2")
    dl_vid("https://youtu.be/zgKazTrhXmI", "st3")
    merge_vids(["st1", "st2", "st3"], "InGame2")

if not os.path.isfile("GameOver.wav"):
    dl_vid("https://youtu.be/J_3Zad-e9f4", "GameOver")
