import { createContext } from "react";
import App from "./App";

export const AppContext = createContext<App | undefined>(undefined);
