import { Outlet, useNavigate, useParams, useMatch } from "react-router-dom";
import { useContext, useEffect, useReducer, useRef, useState } from "react";
import { AppContext } from "../apps";
import { useAuth0 } from "@auth0/auth0-react";
import {
  RoutesContext,
  routesReducer,
  //useRoutesContext,
} from "./RoutesContext";
import {
  ChevronDownIcon,
  ChevronRightIcon,
  EllipsisVerticalIcon,
} from "@heroicons/react/24/solid";
import { useForm } from "react-hook-form";
import { ensureSuccess } from "../../utils/fetchUtils";
//import { toast } from "react-toastify";

interface IExplorerItem extends IRouteFolderItem {
  collapsed?: boolean;
  isEditing?: boolean;
}

export default function RouteIndex() {
  const [state, dispatch] = useReducer(routesReducer, {
    routes: [],
    activeRoute: undefined,
  });
  const params = useParams();
  const routeId = params["routeId"] ? parseInt(params["routeId"]) : undefined;

  const newRouteActive = useMatch("/apps/:appId/routes/new/:folderId?");
  const editRouteActive = useMatch("/apps/:appId/routes/:routeId");
  const logPageActive = useMatch("/apps/:appId/routes/:routeId/logs");

  return (
    <RoutesContext.Provider value={{ state, dispatch }}>
      <div className="flex h-full">
        <div className="w-64 border-r p-1">
          <RouteExplorer />
        </div>
        <div className="flex-1">
          {newRouteActive || editRouteActive || logPageActive ? (
            <div className="">
              <Outlet key={routeId} />
            </div>
          ) : (
            <div className="flex h-screen items-center justify-center">
              Click New button to create new route or click route to edit.
            </div>
          )}
        </div>
      </div>
    </RoutesContext.Provider>
  );
}

function RouteExplorer() {
  const { getAccessTokenSilently } = useAuth0();
  const navigate = useNavigate();

  const app = useContext(AppContext)!;
  const [explorerItems, setExplorerItems] = useState<IExplorerItem[]>([]);

  useEffect(() => {
    const getRoutes = async () => {
      const accessToken = await getAccessTokenSilently();
      fetch(`/api/apps/${app.id}/routes`, {
        headers: {
          Authorization: `Bearer ${accessToken}`,
        },
      })
        .then((res) => res.json() as Promise<IRouteFolderItem[]>)
        .then((routes) => {
          setExplorerItems(routes);
        });
    };
    getRoutes();
  }, []);

  //#region Action Menu
  const [showActionMenu, setShowActionMenu] = useState(false);
  const actionMenuItemRef = useRef<IExplorerItem>();
  const actionMenuRef = useRef<HTMLDivElement>(null);
  const actionMenuClientXRef = useRef<number>();
  const actionMenuClientYRef = useRef<number>();

  function calculateActionMenuTop() {
    if (!actionMenuRef.current || !actionMenuClientYRef.current) {
      return undefined;
    }
    return actionMenuClientYRef.current - actionMenuRef.current.offsetTop;
  }
  function calculateActionMenuRight() {
    if (!actionMenuClientXRef.current) {
      return undefined;
    }
    return 20;
  }

  const handleActionMenuClick = (
    item: IExplorerItem,
    clientX: number,
    clientY: number,
  ) => {
    actionMenuClientXRef.current = clientX;
    actionMenuClientYRef.current = clientY;
    actionMenuItemRef.current = item;
    setShowActionMenu(true);
  };

  //#endregion

  const handleNewRouteClick = async () => {
    navigate(`new/${actionMenuItemRef.current!.id}`);
    setExplorerItems((items) => {
      const newRoute: IExplorerItem = {
        type: "Route",
        id: -1,
        parentId: actionMenuItemRef.current!.id,
        route: {
          name: "New Route",
          method: "GET",
          path: "/new-route",
          status: "Active",
        },
        folder: null,
        collapsed: false,
      };
      return [...items, newRoute];
    });
  };

  const handleNewFolderClick = async (atRoot: boolean = false) => {
    const parentId = atRoot ? null : actionMenuItemRef.current!.id;
    setExplorerItems((items) => {
      const newFolder: IExplorerItem = {
        type: "Folder",
        id: -1,
        parentId: parentId,
        route: null,
        folder: {
          name: "new folder",
        },
        collapsed: false,
        isEditing: true,
      };
      return [...items, newFolder];
    });
  };

  const handleFolderRenameClick = (folder: IExplorerItem) => {
    setExplorerItems((items) => {
      return items.map((item) => {
        if (item.type === "Folder" && item.id === folder.id) {
          return {
            ...item,
            isEditing: true,
          };
        }
        return item;
      });
    });
  };

  const handleFolderNameSubmit = async (
    folder: IExplorerItem,
    name: string,
  ) => {
    const isNewFolder = folder.id === -1;
    const accessToken = await getAccessTokenSilently();
    if (isNewFolder) {
      const res = await fetch(`/api/apps/${app.id}/routes/folders`, {
        method: "POST",
        headers: {
          "Content-Type": "application/json",
          Authorization: `Bearer ${accessToken}`,
        },
        body: JSON.stringify({
          name: name,
          parentId: folder.parentId,
        }),
      });
      ensureSuccess(res);
      setExplorerItems((items) => {
        return items.map((item) => {
          if (item.id === folder.id) {
            return {
              ...item,
              id: parseInt(res.headers.get("Location")!),
              folder: {
                name: name,
              },
              isEditing: false,
            };
          }
          return item;
        });
      });
    } else {
      const res = await fetch(
        `/api/apps/${app.id}/routes/folders/${folder.id}/rename`,
        {
          method: "PATCH",
          headers: {
            "Content-Type": "application/json",
            Authorization: `Bearer ${accessToken}`,
          },
          body: JSON.stringify({
            name: name,
          }),
        },
      );
      ensureSuccess(res);
      setExplorerItems((items) => {
        return items.map((item) => {
          if (item.id === folder.id) {
            return {
              ...item,
              folder: {
                name: name,
              },
              isEditing: false,
            };
          }
          return item;
        });
      });
    }
  };

  const handleDuplicateClick = async () => {
    const { type, id } = actionMenuItemRef.current!;
    const accessToken = await getAccessTokenSilently();
    const url =
      type === "Route"
        ? `/api/apps/${app.id}/routes/${id}/clone`
        : `/api/apps/${app.id}/routes/folders/${id}/duplicate`;

    const res = await fetch(url, {
      method: "POST",
      headers: {
        Authorization: `Bearer ${accessToken}`,
      },
    });
    ensureSuccess(res);
    if (type === "Folder") {
      const newItems = (await res.json()) as IRouteFolderItem[];
      setExplorerItems((items) => {
        return items.concat(
          newItems.map((item) => {
            return {
              ...item,
              collapsed: false,
            };
          }),
        );
      });
    }
  };
  const handleDeleteClick = async () => {
    const { type, id } = actionMenuItemRef.current!;
    if (confirm(`Are you sure want to delete this ${type.toLowerCase()}?`)) {
      console.log("delete", type, id);
      const accessToken = await getAccessTokenSilently();
      const url =
        type === "Route"
          ? `/api/apps/${app.id}/routes/${id}`
          : `/api/apps/${app.id}/routes/folders/${id}`;

      const res = await fetch(url, {
        method: "DELETE",
        headers: {
          Authorization: `Bearer ${accessToken}`,
        },
      });
      ensureSuccess(res);
      setExplorerItems((nodes) => {
        if (type === "Route") {
          return nodes.filter(
            (node) => node.type !== "Route" && node.id !== id,
          );
        }

        let deleteItems = getFolderItems(actionMenuItemRef.current!);
        return nodes.filter((node) => {
          return !deleteItems.some((item) => item.id === node.id);
        });

        function getFolderItems(folder: IExplorerItem) {
          var items = nodes.filter((node) => node.parentId === folder.id);
          for (const item of items) {
            if (item.type === "Folder") {
              items = items.concat(getFolderItems(item));
            }
          }
          return items.concat(folder);
        }
      });
    }
  };

  const handleFolderClick = (folder: IExplorerItem) => {
    setExplorerItems((items) => {
      return items.map((item) => {
        if (item.type === "Folder" && item.id === folder.id) {
          return {
            ...item,
            collapsed: !item.collapsed,
          };
        }
        return item;
      });
    });
  };

  const handleRouteClick = (route: IExplorerItem) => {
    navigate(route.id.toString());
  };

  const renderTree = (nodes: IExplorerItem[], parentId: number | null) => {
    return (
      <>
        {nodes
          .filter((n) => n.parentId === parentId)
          .map((node) => (
            <div style={{ paddingLeft: parentId === null ? 0 : 20 }}>
              {node.type === "Folder" ? (
                <>
                  <FolderItem
                    item={node}
                    onClick={() => handleFolderClick(node)}
                    onActionMenuClick={(clientX, clientY) =>
                      handleActionMenuClick(node, clientX, clientY)
                    }
                    onNameSubmit={(name) => handleFolderNameSubmit(node, name)}
                  />
                  {node.collapsed ? null : renderTree(nodes, node.id)}
                </>
              ) : (
                <RouteItem
                  item={node}
                  onClick={() => handleRouteClick(node)}
                  onActionMenuClick={(clientX, clientY) =>
                    handleActionMenuClick(node, clientX, clientY)
                  }
                />
              )}
            </div>
          ))}
      </>
    );
  };

  return (
    <div>
      <div ref={actionMenuRef} className="relative">
        {showActionMenu && (
          <ActionMenu
            top={calculateActionMenuTop()!}
            right={calculateActionMenuRight()!}
            node={actionMenuItemRef.current!}
            onNewRouteClick={handleNewRouteClick}
            onNewFolderClick={() => handleNewFolderClick(false)}
            onFolderRenameClick={() =>
              handleFolderRenameClick(actionMenuItemRef.current!)
            }
            onDuplicateClick={handleDuplicateClick}
            onDeleteClick={handleDeleteClick}
            onOutSideClick={() => {
              setShowActionMenu(false);
            }}
          />
        )}
      </div>
      <div className="flex justify-center space-x-2">
        <button
          type="button"
          onClick={() => {
            navigate("new");
          }}
          className="bg-primary px-2 py-1 text-white enabled:hover:bg-cyan-700 disabled:opacity-50"
        >
          New Route
        </button>
        <button
          type="button"
          onClick={() => handleNewFolderClick(true)}
          className="bg-primary px-2 py-1 text-white enabled:hover:bg-cyan-700 disabled:opacity-50"
        >
          New Folder
        </button>
      </div>

      <hr className="my-1" />
      {renderTree(explorerItems, null)}
    </div>
  );
}

function RouteItem({
  item: { route },
  onClick,
  onActionMenuClick,
}: {
  item: IExplorerItem;
  onActionMenuClick?: (buttonLeft: number, buttonTop: number) => void;
  onClick: () => void;
}) {
  if (!route) {
    return null;
  }

  const methodTextColors = new Map<string, string>([
    ["GET", "text-sky-500"],
    ["POST", "text-orange-500"],
    ["PUT", "text-green-500"],
    ["DELETE", "text-red-500"],
    ["PATCH", "text-yellow-500"],
  ]);

  const handleActionMenuClick = (
    e: React.MouseEvent<HTMLButtonElement, MouseEvent>,
  ) => {
    e.stopPropagation();
    if (onActionMenuClick) {
      onActionMenuClick(e.currentTarget.offsetLeft, e.currentTarget.offsetTop);
    }
  };

  return (
    <div
      className="group flex items-center p-0.5"
      style={{ cursor: "pointer" }}
      onClick={onClick}
    >
      <span
        className={`me-1 font-semibold ${methodTextColors.get(
          route.method.toUpperCase(),
        )}`}
        style={{ fontSize: "0.65rem" }}
      >
        {route.method}
      </span>
      <span className="overflow-hidden text-ellipsis whitespace-nowrap text-sm">
        {route.name}
      </span>
      <button
        type="button"
        onClick={(e) => handleActionMenuClick(e)}
        className="ms-auto hidden group-hover:block"
      >
        <EllipsisVerticalIcon className="h-4 w-4 text-gray-600" />
      </button>
      {route.status === "Blocked" && (
        <small className="ms-auto text-sm text-red-600">Blocked</small>
      )}
    </div>
  );
}

function FolderItem({
  item: { folder, collapsed, isEditing },
  onActionMenuClick,
  onClick,
  onNameSubmit,
}: {
  item: IExplorerItem;
  onActionMenuClick: (buttonLeft: number, buttonTop: number) => void;
  onClick: () => void;
  onNameSubmit: (name: string) => void;
}) {
  if (!folder) {
    return null;
  }

  const handleClick = (e: React.MouseEvent<HTMLButtonElement, MouseEvent>) => {
    e.stopPropagation();
    onActionMenuClick(e.currentTarget.offsetLeft, e.currentTarget.offsetTop);
  };

  const {
    register,
    handleSubmit,
    formState: { errors },
  } = useForm<{ name: string }>();

  const onSubmit = (data: { name: string }) => {
    onNameSubmit(data.name);
  };
  return !isEditing ? (
    <div
      className="group flex items-center p-0.5"
      style={{ cursor: "pointer" }}
      onClick={onClick}
    >
      {collapsed ? (
        <ChevronRightIcon className="me-1 h-4 w-4" />
      ) : (
        <ChevronDownIcon className="me-1 h-4 w-4" />
      )}
      <span className="overflow-hidden text-ellipsis whitespace-nowrap text-sm">
        {folder.name}
      </span>
      <button
        type="button"
        onClick={(e) => handleClick(e)}
        className="ms-auto hidden group-hover:block"
      >
        <EllipsisVerticalIcon className="h-4 w-4 text-gray-600" />
      </button>
    </div>
  ) : (
    <div className="flex items-center p-0.5">
      <form onSubmit={handleSubmit(onSubmit)}>
        <input
          type="text"
          {...register("name", { required: "Name is required" })}
          className="w-full border"
          defaultValue={folder.name}
        />
        {errors.name && (
          <span className="text-red-500">{errors.name.message}</span>
        )}
      </form>
    </div>
  );
}

function ActionMenu({
  node,
  top,
  right,
  onNewRouteClick,
  onNewFolderClick,
  onDuplicateClick,
  onOutSideClick,
  onDeleteClick,
  onFolderRenameClick,
}: {
  node: IRouteFolderItem;
  top: number;
  right: number;
  onNewRouteClick(): void;
  onNewFolderClick(): void;
  onDuplicateClick(): void;
  onDeleteClick(): void;
  onOutSideClick(): void;
  onFolderRenameClick(): void;
}) {
  const ref = useRef<HTMLDivElement>(null);
  const windowClickHandler = () => {
    onOutSideClick();
  };
  useEffect(() => {
    window.addEventListener("click", windowClickHandler);

    return () => {
      window.removeEventListener("click", windowClickHandler);
    };
  }, []);
  return (
    <>
      <div
        style={{ top: `${top}px`, right: `${right}px` }}
        ref={ref}
        className="absolute w-[80%] border bg-white shadow"
      >
        <ul>
          {node.type === "Folder" && (
            <>
              <li className="p-1.5">
                <button
                  type="button"
                  onClick={onNewRouteClick}
                  className="w-full text-left"
                >
                  New Route
                </button>
              </li>
              <li className="p-1.5">
                <button
                  type="button"
                  onClick={onNewFolderClick}
                  className="w-full text-left"
                >
                  New Folder
                </button>
              </li>
            </>
          )}
          <li className="p-1.5">
            <button
              type="button"
              onClick={onDuplicateClick}
              className="w-full text-left"
            >
              Duplicate
            </button>
          </li>
          {node.type === "Folder" && (
            <li className="p-1.5">
              <button
                type="button"
                onClick={onFolderRenameClick}
                className="w-full text-left"
              >
                Rename
              </button>
            </li>
          )}
          <hr />
          <li className="p-1.5">
            <button
              type="button"
              className="w-full text-left text-red-500"
              onClick={onDeleteClick}
            >
              Delete
            </button>
          </li>
        </ul>
      </div>
    </>
  );
}
