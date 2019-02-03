import { DataAction } from "./data/actions";
import { UIAction } from "./ui/actions";

export type RootAction = DataAction | UIAction;