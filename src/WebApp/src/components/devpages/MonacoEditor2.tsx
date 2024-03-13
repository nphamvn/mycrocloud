import { useEffect, useRef } from "react";
import * as monaco from "monaco-editor/esm/vs/editor/editor.api";
//https://github.com/Microsoft/monaco-editor/issues/604#issuecomment-344214706
export function MonacoEditor2() {
  const monacoEl = useRef(null);
  const editorRef = useRef<monaco.editor.IStandaloneCodeEditor>();
    
  useEffect(() => {
    console.log("useEffect. editorRef.current: ", editorRef.current)
    if (editorRef.current) {
      console.log("useEffect. editorRef.current.dispose()")
      editorRef.current.dispose();
    }
    console.log("useEffect. monaco.editor.create")
    editorRef.current = monaco.editor.create(monacoEl.current!, {
      value: "console.log('Hi')",
      language: "javascript"
    })

    return () => {
      console.log("useEffect. cleanup. editorRef.current: ", editorRef.current)
      if (editorRef.current) {
        console.log("useEffect. cleanup. editorRef.current.dispose()")
        editorRef.current.dispose();
      }
    }
  }, []);

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
