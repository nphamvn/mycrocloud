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

interface ExplorerItem {
  type: "Route" | "Folder";
  id: number;
  parentId: number | null;
  route: IExplorerItemRoute | null;
  folder: {
    name: string;
  } | null;
  folderCollapsed?: boolean;
}

interface IExplorerItemRoute {
  id: number;
  name: string;
  method: string;
  path: string;
  status: string;
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
          <TreeNode />
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

function TreeNode() {
  const { getAccessTokenSilently } = useAuth0();
  const navigate = useNavigate();

  const app = useContext(AppContext)!;
  const [nodes, setNodes] = useState<ExplorerItem[]>([]);

  useEffect(() => {
    const getRoutes = async () => {
      const accessToken = await getAccessTokenSilently();
      fetch(`/api/apps/${app.id}/routes/v2`, {
        headers: {
          Authorization: `Bearer ${accessToken}`,
        },
      })
        .then((res) => res.json())
        .then((routes) => {
          console.log(routes);
          setNodes(routes);
        });
    };
    getRoutes();
  }, []);

  const [showActionMenu, setShowActionMenu] = useState(false);
  const actionMenuNodeRef = useRef<ExplorerItem>();

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
    route: ExplorerItem,
    clientX: number,
    clientY: number,
  ) => {
    actionMenuClientXRef.current = clientX;
    actionMenuClientYRef.current = clientY;
    actionMenuNodeRef.current = route;
    setShowActionMenu(true);
  };

  const handleNewRouteClick = async () => {
    navigate(`new/${actionMenuNodeRef.current!.id}`);

    setNodes((nodes) => {
      const newRoute: ExplorerItem = {
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
      };
      return [...nodes, newRoute];
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
        setNodes((nodes) => {
          const newFolder: ExplorerItem = {
            type: "Folder",
            id: parseInt(res.headers.get("Location")!),
            parentId: actionMenuNodeRef.current!.id,
            route: null,
            folder: {
              name: folderName,
            },
          };
          return [...nodes, newFolder];
        });
      } else {
        console.log("Folder creation failed");
      }
    }
  };

  const handleDuplicateClick = async () => {
    // const accessToken = await getAccessTokenSilently();
    // const res = await fetch(
    //   `/api/apps/${app.id}/routes/${actionMenuRoute!.id}/clone`,
    //   {
    //     method: "POST",
    //     headers: {
    //       Authorization: `Bearer ${accessToken}`,
    //     },
    //   },
    // );
    // const newRoute = (await res.json()) as IRoute;
    // if (res.ok) {
    //   toast.success("Route cloned");
    //   dispatch({ type: "ADD_ROUTE", payload: newRoute });
    // }
  };
  const handleDeleteClick = async () => {
    const type = actionMenuNodeRef.current!.folder ? "folder" : "route";
    if (confirm(`Are you sure want to delete this ${type}?`)) {
      const accessToken = await getAccessTokenSilently();
      const res = await fetch(
        `/api/apps/${app.id}/routes/${actionMenuNodeRef.current!.id}`,
        {
          method: "DELETE",
          headers: {
            Authorization: `Bearer ${accessToken}`,
          },
        },
      );
      if (res.ok) {
        setNodes((nodes) => {
          return nodes.filter(
            (node) =>
              node.type !== actionMenuNodeRef.current!.type &&
              node.id !== actionMenuNodeRef.current!.id,
          );
        });
        // if (actionMenuRoute!.id === activeRoute?.id) {
        //   navigate("./");
        // }
      }
    }
  };

  const renderTree = (
    nodes: ExplorerItem[],
    parentId: number | null,
    isRoot: boolean = false,
  ) => {
    console.log("rendering tree", nodes, parentId);
    return (
      <>
        {nodes
          .filter((node) => node.parentId === parentId)
          .map((node) => {
            console.log("rendering node", node);
            return (
              <div key={node.id} style={{ marginLeft: isRoot ? 0 : 20 }}>
                {node.folder ? (
                  <FolderItem
                    folder={node.folder}
                    collapsed={node.folderCollapsed!}
                    onActionMenuClick={(buttonLeft, buttonTop) =>
                      handleActionMenuClick(node, buttonLeft, buttonTop)
                    }
                  />
                ) : (
                  <RouteItem
                    route={node.route}
                    id={node.id}
                    onActionMenuClick={(buttonLeft, buttonTop) =>
                      handleActionMenuClick(node, buttonLeft, buttonTop)
                    }
                  />
                )}
                {renderTree(nodes, node.id)}
              </div>
            );
          })}
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
              actionMenuNodeRef.current = undefined;
              setShowActionMenu(false);
            }}
          />
        )}
      </div>
      {renderTree(nodes, null, true)}
    </div>
  );
}

function RouteItem({
  route,
  id,
  onActionMenuClick,
}: {
  route: ExplorerItem["route"];
  id: number;
  onActionMenuClick(buttonLeft: number, buttonTop: number): void;
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
    onActionMenuClick(e.currentTarget.offsetLeft, e.currentTarget.offsetTop);
  };

  return (
    <div
      className="group flex items-center p-0.5"
      style={{ cursor: "pointer" }}
      onClick={() => {
        navigate(id.toString());
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
  folder,
  collapsed,
  onActionMenuClick,
}: {
  folder: ExplorerItem["folder"];
  collapsed: boolean;
  onActionMenuClick(buttonLeft: number, buttonTop: number): void;
}) {
  if (!folder) {
    return null;
  }

  const handleClick = (e: React.MouseEvent<HTMLButtonElement, MouseEvent>) => {
    e.stopPropagation();
    onActionMenuClick(e.currentTarget.offsetLeft, e.currentTarget.offsetTop);
  };

  return (
    <div
      className="group flex items-center p-0.5"
      style={{ cursor: "pointer" }}
    >
      {collapsed ? (
        <ChevronRightIcon
          onClick={() => {
            console.log(collapsed);
          }}
          className="h-4 w-4"
        />
      ) : (
        <ChevronDownIcon
          onClick={() => {
            console.log(collapsed);
          }}
          className="h-4 w-4"
        />
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
  node: ExplorerItem;
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
