import React from "react";
import { AppHeader } from "../components/Header";

export const TestPage: React.FC = () => {
    return (
        <div style={{display: "grid", width: '100vw', height: '100vh', gridTemplateRows: 'auto 1fr', gridTemplateColumns: 'repeat(3, 1fr)'}}>
            <div style={{gridColumn: '1/4', gridRow: '1/2'}}>
                <AppHeader/>
            </div>
            <div style={{gridColumn: '2/3', gridRow: '2/3'}}>
                <div style={{textAlign: 'center'}}>Test Page</div>
            </div>
        </div>
    )
};