## Ollama

See [ollama/README.md](ollama/README.md) for installation, CLI usage, and cURL examples.

## LM Studio

1. Download and install [LM Studio](https://lmstudio.ai/).
2. Load a model (e.g. `google/gemma-4-e2b`) and start the local server on port 1234.
3. Run the demo app with the `--lmstudio` flag:

```bash
cd DemoApp
dotnet run -- --lmstudio
```