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
            right: '2%',
            display: 'flex',
            flexDirection: 'column',
            alignItems: 'center'
        }}>
            <div>User menu</div>
            <div>{authStore.user?.displayName}</div>
            <div>
                <button onClick={logOutHandle}>Log out</button>
            </div>
        </div>
    );
};