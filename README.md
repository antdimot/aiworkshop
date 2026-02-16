# AI Development Workshop

Goal of this repository is to collect some examples for local inference.

### __Ollama installation__
```bash
winget install --id Ollama.Ollama
```

### __Using Ollama CLI__
Run model (interactive mode)
```bash
ollama run phi4-mini:latest
```
Run model (single request)
```bash
ollama run phi4-mini:latest "hello"
```
Show model info
```bash
ollama show phi4-mini
```
Get the list of local models
```bash
ollama list
```
Get the list of models run
```bash
ollama ps
```
Pull model from remote registry (default ollama)
```bash
ollama pull gemma3:4b
```
Remove a model
```bash
ollama rm gemma3:4b
```
Get tokens performance results (useful for cost estimation)
```bash
ollama run phi4-mini --verbose "Can you make a simple example of rest api handler only get method? No explanation required."
```
Build and run a custom model
```bash
ollama create phi4-mini-custom -f Modelfile

ollama run phi4-mini-custom --verbose "Can you make a simple example of rest api handler only get method? No explanation required."
```

|__cURL request__
```powershell
$a = curl http://localhost:11434/api/chat -d '{
  "model": "phi4-mini",
  "messages": [{
    "role": "user",
    "content": "Hello"
  }],
  "stream": false
}' | ConvertFrom-Json
```



### __[Ollama .Net application](./dotnet)__