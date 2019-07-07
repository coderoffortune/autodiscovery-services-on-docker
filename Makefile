network_up:
	docker network create tools_network

network_down:
	docker network rm tools_network

dashboard_up:
	cd dashboard && \
	docker build -t dashboard . && \
	docker run -itd --rm --network=tools_network --name dashboard -v /var/run/docker.sock:/var/run/docker.sock -p 5000:80 dashboard && \
	cd ..

dashboard_down:
	docker stop dashboard

tool_up:
	cd $(tool) && \
	docker build -t $(tool) . && \
	docker run -itd --rm --name $(tool) --network=tools_network --label tools.autodiscovery=$(config_url) -p $(port):80 $(tool) && \
	cd ..

tool_down: 
	docker stop $(tool)

tool1_up:
	make tool_up tool=tool1 port=8080 config_url=http://localhost:8080/autodiscovery/config.json

tool1_down:
	make tool_down tool=tool1

tool2_up:
	make tool_up tool=tool2 port=8081 config_url=http://localhost:8081/autodiscovery.json

tool2_down:
	make tool_down tool=tool2

setup:
	make network_up
	make dashboard_up
	make tool1_up
	make tool2_up

clean:
	make dashboard_down
	make tool1_down
	make tool2_down
	make network_down
