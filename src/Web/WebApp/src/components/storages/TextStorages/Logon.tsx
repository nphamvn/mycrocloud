import { useAuth0 } from "@auth0/auth0-react";
import { useParams } from "react-router-dom";
import { AppContext } from "../../apps/AppContext";
import { useContext, useEffect, useRef, useState } from "react";
import ITextStorage from "./ITextStorage";
import * as monaco from "monaco-editor/esm/vs/editor/editor.api";
import { Modal } from "flowbite-react";

export default function Logon() {
  const { getAccessTokenSilently } = useAuth0();
  const app = useContext(AppContext)!;
  const storageId = useParams()["storageId"]!;
  const [storage, setStorage] = useState<ITextStorage>();

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
  const languages = monaco.languages.getLanguages();
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
  const [openWriteConfirmModal, setOpenWriteConfirmModal] = useState(false);

  return (
    <div className="p-3">
      <h1 className="font-bold">
        Logon to storage{" "}
        <span className="italic text-primary">{storage?.name}</span>
      </h1>
      <div className="mt-2">
        <div className="flex space-x-1">
          <button
            type="button"
            onClick={handleReadContentClick}
            className="bg-primary px-5 py-1 text-white"
          >
            Read Content
          </button>
          <button
            type="button"
            onClick={() => setOpenWriteConfirmModal(true)}
            className="bg-primary px-5 py-1 text-white"
          >
            Write Content
          </button>
        </div>
        <div className="mt-2">
          <div className="flex">
            <div>Content</div>
            <div className="ms-auto">
              <label>Language</label>
              <select
                onChange={(e) => {
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
          <div
            className="mt-2"
            style={{ height: "200px", width: "100%" }}
            ref={contentEditorRef}
          ></div>
        </div>
      </div>
      {openWriteConfirmModal && (
        <WriteConfirmModal
          storage={storage!}
          onOk={() => {
            handleWriteContentClick();
            setOpenWriteConfirmModal(false);
          }}
          onCancel={() => {
            setOpenWriteConfirmModal(false);
          }}
        />
      )}
    </div>
  );
}

function WriteConfirmModal({
  storage,
  onOk,
  onCancel,
}: {
  storage: ITextStorage;
  onOk: () => void;
  onCancel: () => void;
}) {
  const [confirmName, setConfirmName] = useState("");
  return (
    <Modal show={true} onClose={onCancel}>
      <div className="p-2">
        <div className="border-b font-semibold">Confirm onverwrite content</div>
        <div className="p-2">
          <p className="">
            Are you sure you want to overwrite the content of the storage?
          </p>
          <div className="mt-2">
            <p className="">Please type the name of the storage to confirm.</p>
            <input
              type="text"
              onChange={(e) => setConfirmName(e.target.value)}
              className="w-full border px-2 py-0.5 text-slate-900"
              placeholder={storage.name}
            />
          </div>
        </div>
        <div className="mt-2 flex space-x-1 border-t p-2">
          <button
            type="button"
            disabled={confirmName !== storage.name}
            onClick={onOk}
            className="bg-primary px-5 py-1 text-white disabled:opacity-50"
          >
            Ok
          </button>
          <button
            type="button"
            onClick={onCancel}
            className="bg-gray-500 px-3 py-1 text-white"
          >
            Cancel
          </button>
        </div>
      </div>
    </Modal>
  );
}
