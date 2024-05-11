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
//import { toast } from "react-toastify";

interface IRouteFolderItem {
  type: "Route" | "Folder";
  id: number;
  parentId: number | null;
  route: IRouteItem | null;
  folder: IFolderItem | null;
}

interface IExplorerItem extends IRouteFolderItem {
  level: number;
  collapsed: boolean;
}

interface IRouteItem {
  id: number;
  name: string;
  method: string;
  path: string;
  status: string;
}

interface IFolderItem {
  name: string;
}

export default function RouteIndex() {
  const app = useContext(AppContext)!;
  const [state, dispatch] = useReducer(routesReducer, {
    routes: [],
    activeRoute: undefined,
  });
  const { getAccessTokenSilently } = useAuth0();
  const navigate = useNavigate();
  const params = useParams();
  const routeId = params["routeId"] ? parseInt(params["routeId"]) : undefined;

  const handleNewFolderClick = async () => {
    const folderName = prompt("Enter folder name");
    if (folderName) {
      const accessToken = await getAccessTokenSilently();
      const res = await fetch(`/api/apps/${app.id}/routes/folders`, {
        method: "POST",
        headers: {
          "Content-Type": "application/json",
          Authorization: `Bearer ${accessToken}`,
        },
        body: JSON.stringify({ name: folderName }),
      });
      if (res.ok) {
        console.log("Folder created");
      } else {
        console.log("Folder creation failed");
      }
    }
  };
  const newRouteActive = useMatch("/apps/:appId/routes/new/:folderId?");
  const editRouteActive = useMatch("/apps/:appId/routes/:routeId");
  const logPageActive = useMatch("/apps/:appId/routes/:routeId/logs");

  return (
    <RoutesContext.Provider value={{ state, dispatch }}>
      <div className="flex h-full">
        <div className="w-48 border-r p-1">
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
              onClick={handleNewFolderClick}
              className="bg-primary px-2 py-1 text-white enabled:hover:bg-cyan-700 disabled:opacity-50"
            >
              New Folder
            </button>
          </div>

          <hr className="my-1" />
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
      fetch(`/api/apps/${app.id}/routes/v2`, {
        headers: {
          Authorization: `Bearer ${accessToken}`,
        },
      })
        .then((res) => res.json() as Promise<IRouteFolderItem[]>)
        .then((routes) => {
          setExplorerItems(
            routes.map((route) => {
              return {
                ...route,
                level: calculateLevel(route, routes),
                collapsed: false,
              };

              function calculateLevel(
                route: IRouteFolderItem,
                routes: IRouteFolderItem[],
              ): number {
                if (route.parentId === null) {
                  return 0;
                }
                const parent = routes.find((r) => r.id === route.parentId);

                if (!parent) {
                  throw new Error("Parent not found for route");
                }

                var parentLevel = calculateLevel(parent, routes);
                return parentLevel + 1;
              }
            }),
          );
        });
    };
    getRoutes();
  }, []);

  const [showActionMenu, setShowActionMenu] = useState(false);
  const actionMenuNodeRef = useRef<IExplorerItem>();

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
    actionMenuNodeRef.current = item;
    console.log("actionMenuNodeRef", actionMenuNodeRef.current);
    setShowActionMenu(true);
  };

  const handleNewRouteClick = async () => {
    navigate(`new/${actionMenuNodeRef.current!.id}`);

    setExplorerItems((items) => {
      const newRoute: IExplorerItem = {
        type: "Route",
        id: -1,
        parentId: actionMenuNodeRef.current!.id,
        route: {
          id: -1,
          name: "New Route",
          method: "GET",
          path: "/new-route",
          status: "Active",
        },
        folder: null,
        level: actionMenuNodeRef.current!.level + 1,
        collapsed: false,
      };
      return [...items, newRoute];
    });
  };

  const handleNewFolderClick = async () => {
    const folderName = prompt("Enter folder name");
    if (folderName) {
      const accessToken = await getAccessTokenSilently();
      const res = await fetch(`/api/apps/${app.id}/routes/folders`, {
        method: "POST",
        headers: {
          "Content-Type": "application/json",
          Authorization: `Bearer ${accessToken}`,
        },
        body: JSON.stringify({
          name: folderName,
          parentId: actionMenuNodeRef.current!.id,
        }),
      });
      if (res.ok) {
        setExplorerItems((items) => {
          const newFolder: IExplorerItem = {
            type: "Folder",
            id: parseInt(res.headers.get("Location")!),
            parentId: actionMenuNodeRef.current!.id,
            route: null,
            folder: {
              name: folderName,
            },
            collapsed: false,
            level: actionMenuNodeRef.current!.level + 1,
          };
          return [...items, newFolder];
        });
      } else {
        console.log("Folder creation failed");
      }
    }
  };

  const handleDuplicateClick = async () => {
    const { type, id } = actionMenuNodeRef.current!;
    console.log("duplicate", type, id);
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
    if (res.ok) {
      if (type === "Folder") {
        const newFolderId = parseInt(res.headers.get("Location")!);
        const newItems = (await res.json()) as IRouteFolderItem[];
        const newFolder = newItems.find((item) => item.id === newFolderId)!;
        const newFolderLevel = explorerItems.find(
          (item) => item.parentId === newFolder.parentId,
        )!.level;

        const newExplorerItems: IExplorerItem[] = newItems.map((item) => {
          return {
            ...item,
            level: calculateLevel(item, newItems, newFolderId, newFolderLevel),
            collapsed: false,
          };

          function calculateLevel(
            item: IRouteFolderItem,
            items: IRouteFolderItem[],
            topItemId: number,
            topItemLevel: number,
          ): number {
            if (item.id === topItemId) {
              return topItemLevel;
            }
            var parent = items.find((i) => i.id === item.parentId);
            if (!parent) {
              throw new Error("Parent not found");
            }
            var parentLevel = calculateLevel(
              parent,
              items,
              topItemId,
              topItemLevel,
            );
            return parentLevel + 1;
          }
        });

        setExplorerItems((items) => {
          return items.concat(newExplorerItems);
        });
      }
      // toast.success("Route cloned");
      // dispatch({ type: "ADD_ROUTE", payload: newRoute });
    }
  };
  const handleDeleteClick = async () => {
    const { type, id } = actionMenuNodeRef.current!;
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
      if (res.ok) {
        setExplorerItems((nodes) => {
          if (type === "Route") {
            return nodes.filter(
              (node) => node.type !== "Route" && node.id !== id,
            );
          }

          let deleteItems = getFolderItems(actionMenuNodeRef.current!);
          console.log("deleteItems", deleteItems);
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
        // if (actionMenuRoute!.id === activeRoute?.id) {
        //   navigate("./");
        // }
      }
    }
  };

  if (explorerItems.length === 0) {
    return <div>Loading...</div>;
  }

  console.log("nodes", explorerItems);

  const handleFolderCollapseClick = (target: IExplorerItem) => {
    setExplorerItems((items) => {
      return items.map((item) => {
        if (item.type === "Folder" && item.id === target.id) {
          return {
            ...item,
            collapsed: !item.collapsed,
          };
        }
        return item;
      });
    });
  };

  const render = (nodes: IExplorerItem[], parentId: number | null) => {
    return (
      <>
        {nodes
          .filter((n) => n.parentId === parentId)
          .map((node) => (
            <div style={{ paddingLeft: node.level === 0 ? 0 : 20 }}>
              {node.type === "Folder" ? (
                <>
                  <FolderItem
                    item={node}
                    onCollapseClick={() => handleFolderCollapseClick(node)}
                    onActionMenuClick={(clientX, clientY) =>
                      handleActionMenuClick(node, clientX, clientY)
                    }
                  />
                  {node.collapsed ? null : render(nodes, node.id)}
                </>
              ) : (
                <RouteItem
                  item={node}
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
            node={actionMenuNodeRef.current!}
            onNewRouteClick={handleNewRouteClick}
            onNewFolderClick={handleNewFolderClick}
            onDuplicateClick={handleDuplicateClick}
            onDeleteClick={handleDeleteClick}
            onOutSideClick={() => {
              //actionMenuNodeRef.current = undefined;
              setShowActionMenu(false);
            }}
          />
        )}
      </div>
      {render(explorerItems, null)}
    </div>
  );
}

function RouteItem({
  item: { route },
  onActionMenuClick,
}: {
  item: IExplorerItem;
  onActionMenuClick?: (buttonLeft: number, buttonTop: number) => void;
}) {
  if (!route) {
    return null;
  }

  const navigate = useNavigate();

  const methodTextColors = new Map<string, string>([
    ["GET", "text-sky-500"],
    ["POST", "text-orange-500"],
    ["PUT", "text-green-500"],
    ["DELETE", "text-red-500"],
    ["PATCH", "text-yellow-500"],
  ]);

  const handleClick = (e: React.MouseEvent<HTMLButtonElement, MouseEvent>) => {
    e.stopPropagation();
    if (onActionMenuClick) {
      onActionMenuClick(e.currentTarget.offsetLeft, e.currentTarget.offsetTop);
    }
  };

  return (
    <div
      className="group flex items-center p-0.5"
      style={{ cursor: "pointer" }}
      onClick={() => {
        navigate(route.id.toString());
      }}
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
        onClick={(e) => handleClick(e)}
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
  item: { folder, collapsed },
  onActionMenuClick,
  onCollapseClick,
}: {
  item: IExplorerItem;
  onActionMenuClick?: (buttonLeft: number, buttonTop: number) => void;
  onCollapseClick?: () => void;
}) {
  if (!folder) {
    return null;
  }

  const handleClick = (e: React.MouseEvent<HTMLButtonElement, MouseEvent>) => {
    e.stopPropagation();
    if (onActionMenuClick) {
      onActionMenuClick(e.currentTarget.offsetLeft, e.currentTarget.offsetTop);
    }
  };

  return (
    <div
      className="group flex items-center p-0.5"
      style={{ cursor: "pointer" }}
      onClick={() => {
        onCollapseClick && onCollapseClick();
      }}
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
}: {
  node: IRouteFolderItem;
  top: number;
  right: number;
  onNewRouteClick(): void;
  onNewFolderClick(): void;
  onDuplicateClick(): void;
  onDeleteClick(): void;
  onOutSideClick(): void;
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
          {node.folder && (
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
