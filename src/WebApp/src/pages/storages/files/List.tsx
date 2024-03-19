import {
  DocumentIcon,
  EllipsisHorizontalIcon,
  FolderIcon,
} from "@heroicons/react/24/solid";
import React, { useContext, useEffect, useRef, useState } from "react";
import Item, { FolderPathItem } from "./Item";
import { Link, useSearchParams } from "react-router-dom";
import { useAuth0 } from "@auth0/auth0-react";
import { AppContext } from "../../apps";
import { Modal } from "flowbite-react";
import { useForm } from "react-hook-form";

interface PageData {
  items: Item[];
  folderPathItems: FolderPathItem[];
}

type CreateRenameFolderFormInputs = {
  name: string;
};

export default function List() {
  const app = useContext(AppContext)!;
  const { getAccessTokenSilently } = useAuth0();

  const [{ items, folderPathItems }, setPageData] = useState<PageData>({
    items: [],
    folderPathItems: [],
  });

  const [searchParams] = useSearchParams();
  const folderId = searchParams.get("folderId")
    ? parseInt(searchParams.get("folderId")!)
    : undefined;

  useEffect(() => {
    const fetchItems = async (folderId?: number) => {
      const accessToken = await getAccessTokenSilently();
      const res = await fetch(
        folderId
          ? `/api/apps/${app.id}/files?folderId=${folderId}`
          : `/api/apps/${app.id}/files`,
        {
          headers: {
            Authorization: `Bearer ${accessToken}`,
          },
        },
      );
      const items = (await res.json()) as Item[];
      const folderPathItems = JSON.parse(
        res.headers.get("Path-Items")!,
      ) as FolderPathItem[];
      setPageData({
        items: items,
        folderPathItems: folderPathItems,
      });
    };
    fetchItems(folderId);
  }, [folderId]);

  const renameFolder = useRef<Item>();

  const createRenameFolderForm = useForm<CreateRenameFolderFormInputs>();
  const onSumitCreateRenameFolderForm = async (
    inputs: CreateRenameFolderFormInputs,
  ) => {
    const accessToken = await getAccessTokenSilently();
    const isEditMode = renameFolder.current !== undefined;
    let url = `/api/apps/${app.id}/files/folders`;
    let query;
    if (!isEditMode) {
      query = folderId ? `?folderId=${folderId}` : "";
    } else {
      query = `?folderId=${renameFolder.current!.id}`;
    }
    url += query;
    const res = await fetch(url, {
      method: !isEditMode ? "POST" : "PATCH",
      headers: {
        "content-type": "application/json",
        Authorization: `Bearer ${accessToken}`,
      },
      body: JSON.stringify(inputs),
    });

    if (res.ok) {
      setShowCreateRenameFolder(false);
    }
  };
  const [showCreateRenameFolder, setShowCreateRenameFolder] = useState(false);

  const handleRenameClick = (item: Item) => {
    renameFolder.current = item;
    createRenameFolderForm.reset();
    createRenameFolderForm.resetField("name");
    createRenameFolderForm.setValue("name", item.name);
    setShowCreateRenameFolder(true);
  };
  return (
    <div className="p-2">
      <h1>Files</h1>
      <div className="mt-2 flex space-x-1">
        <button className="bg-primary px-2 py-1 text-white">Upload File</button>
        <button
          onClick={() => {
            if (renameFolder.current) {
              renameFolder.current = undefined;
            }
            createRenameFolderForm.reset();
            createRenameFolderForm.resetField("name");
            setShowCreateRenameFolder(true);
          }}
          className="bg-primary px-2 py-1 text-white"
        >
          New Folder
        </button>
      </div>
      <div className="mt-2 flex space-x-1">
        {folderPathItems.map((item, index) => (
          <React.Fragment key={item.id}>
            <Link
              to={`?folderId=${item.id}`}
              className={item.id === folderId ? "font-semibold" : ""}
            >
              {item.name}
            </Link>
            {index < folderPathItems.length - 1 && <span>{">"}</span>}
          </React.Fragment>
        ))}
      </div>
      <table className="w-full">
        <thead>
          <tr>
            <td></td>
            <td>Name</td>
            <td>Size (B)</td>
            <td>Created At</td>
            <td>Actions</td>
          </tr>
        </thead>
        <tbody>
          {items.map((item) => (
            <tr key={item.id}>
              <td>
                {item.type === "File" ? (
                  <DocumentIcon className="h-4 w-4 text-blue-500" />
                ) : (
                  <FolderIcon className="h-4 w-4 text-yellow-500" />
                )}
              </td>
              <td>
                {item.type === "File" ? (
                  <>{item.name}</>
                ) : (
                  <Link to={`?folderId=${item.id}`}>{item.name}</Link>
                )}
              </td>
              <td>{item.size || "-"}</td>
              <td>{new Date(item.createdAt).toDateString()}</td>
              <td>
                <button>
                  <EllipsisHorizontalIcon className="h-4 w-4" />
                </button>
                <button onClick={() => handleRenameClick(item)}>Rename</button>
              </td>
            </tr>
          ))}
        </tbody>
      </table>
      <Modal
        dismissible
        show={showCreateRenameFolder}
        onClose={() => {
          setShowCreateRenameFolder(false);
        }}
      >
        <form
          onSubmit={createRenameFolderForm.handleSubmit(
            onSumitCreateRenameFolderForm,
          )}
        >
          <Modal.Header>Create new folder</Modal.Header>
          <Modal.Body>
            <div>
              <input
                type="text"
                {...createRenameFolderForm.register("name", {
                  required: "Folder name is required",
                })}
                className="w-full border px-2 py-1"
              />
              {createRenameFolderForm.formState.errors.name && (
                <span className="text-red-500">
                  {createRenameFolderForm.formState.errors.name.message}
                </span>
              )}
            </div>
            <div className="mt-2">
              <button type="submit" className="bg-primary px-2 py-1 text-white">
                Create
              </button>
            </div>
          </Modal.Body>
        </form>
      </Modal>
    </div>
  );
}
