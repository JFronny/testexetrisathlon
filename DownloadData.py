import os
import os.path
import youtube_dl
import wave


os.chdir("testexetrisathlon")

def dl_vid(url, outfile, filetype):
    ydl_opts = {
        'outtmpl': outfile,
        'noplaylist': True,
        'continue_dl': True,
        'postprocessors': [{
            'key': 'FFmpegExtractAudio',
            'preferredcodec': filetype }]
    }
    with youtube_dl.YoutubeDL(ydl_opts) as ydl:
        ydl.download([url])

def merge_vids(infiles, outfile):
    #lStream = open("list.txt", "w")
    #for infile in infiles:
    #    lStream.write("file '" + infile + ".wav'\n")
    #lStream.close()
    #os.system("ffmpeg -f concat -i list.txt -c copy " + outfile + ".mp3")
    #os.remove("list.txt")
    #for infile in infiles:
    #    os.remove(infile + ".mp3")
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
    os.system("ffmpeg -y -i " + outfile + ".wav  " + outfile + ".mp3")
    os.remove(outfile + ".wav")

if not os.path.isfile("Intro.mp3"):
    dl_vid("https://youtu.be/U06jlgpMtQs", "Intro", "mp3")
    

if not os.path.isfile("InGame1.mp3"):
    dl_vid("https://youtu.be/hueJrl83sOQ", "st1", "wav")
    dl_vid("https://youtu.be/7gSS4h47rLU", "st2", "wav")
    dl_vid("https://youtu.be/NDjDgvXlfVw", "st3", "wav")
    merge_vids(["st1", "st2", "st3"], "InGame1")

if not os.path.isfile("InGame2.mp3"):
    dl_vid("https://youtu.be/umEDct4BoGc", "st1", "wav")
    dl_vid("https://youtu.be/NVpjt9gHlDw", "st2", "wav")
    dl_vid("https://youtu.be/zgKazTrhXmI", "st3", "wav")
    merge_vids(["st1", "st2", "st3"], "InGame2")

if not os.path.isfile("GameOver.mp3"):
    dl_vid("https://youtu.be/J_3Zad-e9f4", "GameOver", "mp3")
