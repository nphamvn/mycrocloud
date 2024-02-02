import { useAuth0 } from "@auth0/auth0-react";
import { useParams } from "react-router-dom";
import { AppContext } from "../../apps/AppContext";
import { useContext, useEffect, useRef, useState } from "react";
import ITextStorage from "./ITextStorage";
import * as monaco from "monaco-editor/esm/vs/editor/editor.api";

const languages = monaco.languages.getLanguages();

export default function Logon() {
  const { getAccessTokenSilently } = useAuth0();
  const app = useContext(AppContext)!;
  const storageId = useParams()["storageId"]!;
  const [, setStorage] = useState<ITextStorage>();

  useEffect(() => {
    const getStorage = async () => {
      const accessToken = await getAccessTokenSilently();
      const res = await fetch(`/api/apps/${app.id}/textstorages/${storageId}`, {
        headers: {
          Authorization: `Bearer ${accessToken}`,
        },
      });
      const storage = (await res.json()) as ITextStorage;
      setStorage(storage);
    };
    getStorage();
  }, []);

  const contentEditorRef = useRef(null);
  const [contentEditor, setContentEditor] =
    useState<monaco.editor.IStandaloneCodeEditor>();

  useEffect(() => {
    let isMounted = true;
    if (contentEditorRef.current && isMounted) {
      setContentEditor((editor) => {
        if (editor) return editor;
        const instance = monaco.editor.create(contentEditorRef.current!, {
          language: "",
          value: "",
          minimap: {
            enabled: false,
          },
        });
        return instance;
      });
    }

    return () => {
      isMounted = false;
    };
  }, [contentEditorRef.current]);

  const handleContentEditorLanguageChange = (language: string) => {
    if (contentEditor) {
      monaco.editor.setModelLanguage(contentEditor.getModel()!, language);
    }
  };

  const handleReadContentClick = async () => {
    const accessToken = await getAccessTokenSilently();
    const res = await fetch(
      `/api/apps/${app.id}/textstorages/${storageId}/content`,
      {
        headers: {
          Authorization: `Bearer ${accessToken}`,
        },
      },
    );

    const content = await res.text();
    if (contentEditor) {
      contentEditor.setValue(content);
    }
  };

  const handleWriteContentClick = async () => {
    if (!confirm("Are you sure want to overwrite content?")) {
      return;
    }
    const accessToken = await getAccessTokenSilently();
    const res = await fetch(
      `/api/apps/${app.id}/textstorages/${storageId}/content`,
      {
        method: "PUT",
        headers: {
          "content-type": "text/plain",
          Authorization: `Bearer ${accessToken}`,
        },
        body: contentEditor?.getValue(),
      },
    );

    if (res.ok) {
      //
    }
  };
  return (
    <div className="p-3">
      <h1 className="font-bold">Logon to storage</h1>
      <div className="mt-2">
        <button
          type="button"
          onClick={handleReadContentClick}
          className="bg-primary px-5 py-1.5 text-white"
        >
          Read Content
        </button>
        <button
          type="button"
          onClick={handleWriteContentClick}
          className="bg-primary px-5 py-1.5 text-white"
        >
          Write Content
        </button>
        <div className="mt-2">
          <div className="flex">
            <div>Content</div>
            <div className="ms-auto">
              <label>Language</label>
              <select
                onChange={(e) => {
                  console.log(e.target.value);
                  handleContentEditorLanguageChange(e.target.value);
                }}
              >
                {languages.map((l) => (
                  <option key={l.id} value={l.id}>
                    {l.aliases?.[0] || l.id}
                  </option>
                ))}
              </select>
            </div>
          </div>
          <div className="mt-2 h-[200] w-full" ref={contentEditorRef}></div>
        </div>
      </div>
    </div>
  );
}
