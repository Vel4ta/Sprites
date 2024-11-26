import { Component, createRef } from 'react';
import Table from './Table';
import Player from './Player';
import { getSpriteSheetData } from './Utils';


export default class App extends Component {
    static displayName = App.name;

    constructor(props) {
        super(props);
        this.state = { spritesheets: null, loading: true, selection: null };
        this.world_map = createRef(null);
        this.bounds = createRef(null);
        this.position = createRef(null);
    }

    componentDidMount() {
        getSpriteSheetData(
            [
                {
                    id: 3,
                    settings_id: 11
                },
                {
                    id: 1,
                    settings_id: 3
                },
                {
                    id: 1,
                    settings_id: 4
                },
                {
                    id: 1,
                    settings_id: 5
                },
                {
                    id: 1,
                    settings_id: 6
                },
                {
                    id: 2,
                    settings_id: 10
                },
                {
                    id: 2,
                    settings_id: 7
                },
                {
                    id: 2,
                    settings_id: 8
                },
                {
                    id: 2,
                    settings_id: 9
                },
                {
                    id: 0,
                    settings_id: 1
                },
                {
                    id: 0,
                    settings_id: 2
                },
                {
                    id: 0,
                    settings_id: 0
                },
                {
                    id: 4,
                    settings_id: 12
                },
                {
                    id: 4,
                    settings_id: 13
                },
            ]
        ).then(data => data.map(obj => ({ title: obj.title, info: obj })))
            .then(data => this.setState({
                spritesheets: {
                    fruit: data.filter(sheet => sheet.title == "Fruit"),
                    gems: data.filter(sheet => sheet.title == "Gems"),
                    soulder: data.filter(sheet => sheet.title == "Knight Attack"),
                    panda: data.filter(sheet => sheet.title == "Panda"),
                    table: data.filter(sheet => sheet.title == "Table")[0]
                }, loading: false, selection: null
            }));
    }

    static renderSpriteSheetList(spritesheets, setSelection, world_map) {
        return (
            <>
                <Table world_map={world_map} position={{ x: 100, y: 100 }} setSelection={setSelection} spritesheets={spritesheets.panda} display_amount={1} texture={spritesheets.table.info}></Table>
                <Table world_map={world_map} position={{ x: 10, y: 300 }} setSelection={setSelection} spritesheets={spritesheets.fruit} display_amount={1} texture={spritesheets.table.info}></Table>
                <Table world_map={world_map} position={{ x: 200, y: 500 }} setSelection={setSelection} spritesheets={spritesheets.gems} display_amount={2} texture={spritesheets.table.info}></Table>
                <Table world_map={world_map} position={{ x: 400, y: 800 }} setSelection={setSelection} spritesheets={spritesheets.gems} display_amount={2} texture={spritesheets.table.info}></Table>
                <Table world_map={world_map} position={{ x: 700, y: 630 }} setSelection={setSelection} spritesheets={spritesheets.fruit} display_amount={1} texture={spritesheets.table.info}></Table>
                <Table world_map={world_map} position={{ x: 900, y: 400 }} setSelection={setSelection} spritesheets={spritesheets.fruit} display_amount={1} texture={spritesheets.table.info}></Table>
                <Table world_map={world_map} position={{ x: 500, y: 350 }} setSelection={setSelection} spritesheets={spritesheets.gems} display_amount={2} texture={spritesheets.table.info}></Table>
            </>
        );
    }

    render() {
        let contents = this.state.loading
            ? <p><em>Loading... Please refresh once the ASP.NET backend has started. See <a href="https://aka.ms/jspsintegrationreact">https://aka.ms/jspsintegrationreact</a> for more details.</em></p>
            : App.renderSpriteSheetList(this.state.spritesheets, (selection, bounds, position) => {
                this.setState({
                    spritesheets: this.state.spritesheets,
                    loading: this.state.loading,
                    selection: selection
                });
                this.bounds.current = bounds;
                this.position.current = position;
            }, this.world_map);

        return (
            <div>
                <h1 id="spriteTabelLabel" >Sprites</h1>
                {contents}
                {this.bounds.current && this.bounds.current.map.map((box, index) => <div key={index} style={{ width: box.value.width + "px", height: box.value.height + "px", bottom: box.value.y, left: box.value.x, border: "solid black 1px", position: "absolute" }}>{index}</div>)}
                {this.state.selection != null && <Player {...this.state.selection} bounds={this.bounds} position={this.position}>
                </Player>}
            </div>
        );
    }
}
