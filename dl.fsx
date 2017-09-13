open System.Net
open System.IO
open System.Diagnostics

let executeCommand (cmd: string) = 
    let proc = new Process()
    proc.StartInfo.FileName <- "/bin/bash" 
    proc.StartInfo.Arguments <-  sprintf """-c "%s" """ cmd 
    proc.StartInfo.UseShellExecute <-  false
    proc.StartInfo.RedirectStandardOutput <- false
    proc.Start() |> ignore
    proc.WaitForExit() |> ignore

let segStart = 0
let segEnd = 10
let folder = "dest"
let wc = new WebClient()
let dest = sprintf "%s/%s" __SOURCE_DIRECTORY__ folder

for seg = segStart to segEnd do
    let segUrl = sprintf "https://url/seg-%i.ts?" seg
    let filePath = sprintf "%s/%i.ts" dest seg
    wc.DownloadFile(segUrl, filePath)
    ()

let files = ([segStart..segEnd] |> List.map (fun x -> sprintf "%i.ts" x) |> String.concat " ")

sprintf "cd %s; cat %s > all.ts" dest files
    |> executeCommand 
    |> ignore
