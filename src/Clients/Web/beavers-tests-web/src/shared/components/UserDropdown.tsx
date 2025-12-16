import React from "react";
import { useStore } from "../stores";

export const UserDropdown: React.FC = () => {
    const { authStore } = useStore();
    
    const logOutHandle = async (): Promise<void> => {
        await authStore.logout();
    };

    return (
        <div style={{
            position: 'fixed',
            right: '0%'
        }}>
            User menu
            <button onClick={logOutHandle}>Log out</button>
        </div>
    );
};