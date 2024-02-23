import { useEffect, useRef, useState } from "react";
import * as monaco from "monaco-editor/esm/vs/editor/editor.api";

export default function DevPage() {
  return (
    <div>
      <h1>DevPage</h1>
      {false && <MonacoEditor />}
      {false && <ChildComponent />}
      {false && <Dropdown />}
    </div>
  );
}

function MonacoEditor() {
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

function ChildComponent() {
  console.log("ChildComponent");
  const [state, setState] = useState(0);
  useEffect(() => {
    console.log("ChildComponent:useEffect");
    setTimeout(() => {
      setState(1);
    }, 100);
  }, []);
  return <div>ChildComponent. state: {state}</div>;
}

function Dropdown() {
  return (
    <>
      <button
        id="dropdownDefaultButton"
        data-dropdown-toggle="dropdown"
        className="inline-flex items-center rounded-lg bg-blue-700 px-5 py-2.5 text-center text-sm font-medium text-white hover:bg-blue-800 focus:outline-none focus:ring-4 focus:ring-blue-300 dark:bg-blue-600 dark:hover:bg-blue-700 dark:focus:ring-blue-800"
        type="button"
      >
        Dropdown button{" "}
        <svg
          className="ms-3 h-2.5 w-2.5"
          aria-hidden="true"
          xmlns="http://www.w3.org/2000/svg"
          fill="none"
          viewBox="0 0 10 6"
        >
          <path
            stroke="currentColor"
            strokeLinecap="round"
            strokeLinejoin="round"
            strokeWidth="2"
            d="m1 1 4 4 4-4"
          />
        </svg>
      </button>

      <div
        id="dropdown"
        className="z-10 hidden w-44 divide-y divide-gray-100 rounded-lg bg-white shadow dark:bg-gray-700"
      >
        <ul
          className="py-2 text-sm text-gray-700 dark:text-gray-200"
          aria-labelledby="dropdownDefaultButton"
        >
          <li>
            <a
              href="#"
              className="block px-4 py-2 hover:bg-gray-100 dark:hover:bg-gray-600 dark:hover:text-white"
            >
              Dashboard
            </a>
          </li>
          <li>
            <a
              href="#"
              className="block px-4 py-2 hover:bg-gray-100 dark:hover:bg-gray-600 dark:hover:text-white"
            >
              Settings
            </a>
          </li>
          <li>
            <a
              href="#"
              className="block px-4 py-2 hover:bg-gray-100 dark:hover:bg-gray-600 dark:hover:text-white"
            >
              Earnings
            </a>
          </li>
          <li>
            <a
              href="#"
              className="block px-4 py-2 hover:bg-gray-100 dark:hover:bg-gray-600 dark:hover:text-white"
            >
              Sign out
            </a>
          </li>
        </ul>
      </div>
    </>
  );
}
