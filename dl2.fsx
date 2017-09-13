open System
open System.Net
open System.IO
open System.Diagnostics
open System.Collections.Generic
open System.Threading
open System.Linq

let executeCommand (cmd: string) = 
    let proc = new Process()
    proc.StartInfo.FileName <- "/bin/bash" 
    proc.StartInfo.Arguments <-  sprintf """-c "%s" """ cmd 
    proc.StartInfo.UseShellExecute <-  false
    proc.StartInfo.RedirectStandardOutput <- false
    proc.Start() |> ignore
    proc.WaitForExit() |> ignore

let m3u8Url = "https://url/filename.m3u8"
let wc = new WebClient()
let folder = "dest"
let dest = sprintf "%s/%s" __SOURCE_DIRECTORY__ folder
let downloadedSegs = new List<string>()
let mutable nbrSegToDownload = 20
let mutable files = ""

while nbrSegToDownload > 0 do
    let m3u8Content = wc.DownloadString(m3u8Url)
    let segs = m3u8Content.Split('\n') |> Array.filter (fun x -> not <| x.StartsWith("#"))
    for seg in segs do
        let fileName = sprintf "%s.ts" (DateTime.Now.ToString("dd-MM-yy-H-mm-ss-fff"))
        let filePath = sprintf "%s/%s" dest fileName
        if  not <| downloadedSegs.Contains(seg) then
            downloadedSegs.Add(seg)
            try
                wc.DownloadFile(seg, filePath)
                files <- sprintf "%s %s" files fileName
                nbrSegToDownload <- nbrSegToDownload - 1
            with
            | ex -> printf "EXCEPTION %s" filePath
    Thread.Sleep(2000)

sprintf "cd %s; cat %s > all.ts" dest files 
    |> executeCommand 
    |> ignore
