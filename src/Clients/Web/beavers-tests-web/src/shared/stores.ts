import { createContext, useContext } from 'react';
import AuthStore from '../auth/authStore';

// Корневое хранилище объединяет все хранилища приложения
export class RootStore {
  authStore: AuthStore;

  constructor() {
    this.authStore = new AuthStore();
  }
}

// Создаем экземпляр хранилища
export const rootStore = new RootStore();

// Контекст для доступа к хранилищу из компонентов
export const StoreContext = createContext(rootStore);

// Хук для удобного использования хранилища
export const useStore = (): RootStore => useContext(StoreContext);