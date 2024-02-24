import { ChildComponent } from "./ChildComponent";
import { MonacoEditor } from "./MonacoEditor";

const routes = [
  {
    path: "MonacoEditor",
    name: "MonacoEditor",
    component: MonacoEditor,
  },
  {
    path: "ChildComponent",
    name: "ChildComponent",
    component: ChildComponent,
  },
];

export default routes;
