import { useEffect, useRef, useState } from "react";
import * as monaco from "monaco-editor/esm/vs/editor/editor.api";

export function MonacoEditor() {
  const monacoEl = useRef(null);
  const [editor, setEditor] =
    useState<monaco.editor.IStandaloneCodeEditor | null>(null);
  const [value, setValue] = useState("");

  useEffect(() => {
    // Load the Monaco Editor
    setEditor((prevEditor) => {
      if (prevEditor) return prevEditor;

      const editor = monaco.editor.create(monacoEl.current!, {
        value: "",
        language: "javascript",
      });
      return editor;
    });
    // Simulate fetching data
    setTimeout(() => {
      setValue("console.log('Hello, world!')");
    }, 100);
    return () => {
      editor?.dispose();
    };
  }, []);

  useEffect(() => {
    // Set the value of the editor
    if (editor) {
      editor.setValue(value);
    }
  }, [editor, value]);

  return (
    <div>
      <h1>MonacoEditor</h1>
      <div
        ref={monacoEl}
        style={{ height: "200px", width: "100%", border: "1px gray solid" }}
      ></div>
    </div>
  );
}
