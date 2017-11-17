using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Client
{
    class GameObject
    {
        private Transform transform;
        private Spriterendere renderer;

        public Transform Transform { get { return transform; } }
        public Spriterendere Rendere { get { return renderer; } }

        private List<Component> components;

        public GameObject()
        {
            this.components = new List<Component>();
        }

        public GameObject(params Component[] components)
        {
            this.components = new List<Component>(components);
        }

        public void AddComponent(Component component)
        {
            components.Add(component);

            if (component is Transform)
                transform = component as Transform;
            else if (component is Spriterendere)
                renderer = component as Spriterendere;
        }

        public void RemoveComponent(Component component)
        {
            components.Remove(component);

            if (component == transform)
                transform = GetComponent<Transform>();
            else if (component == renderer)
                renderer = GetComponent<Spriterendere>();
        }

        public void RemoveAllComponent<T>() where T : Component
        {
            components.RemoveAll(o => o is T);

            if (typeof(T) == typeof(Transform))
                transform = null;
            else if (typeof(T) == typeof(Spriterendere))
                renderer = null;
        }

        public T GetComponent<T>() where T : Component
        {
            return components.Find(o => o is T) as T;
        }

        public Component GetComponent(Predicate<Component> filter)
        {
            return components.Find(filter);
        }

        public T GetComponent<T>(Predicate<T> filter) where T : Component
        {
            return components.Find(o => o is T && filter(o as T)) as T;
        }
        
        public T[] GetComponents<T>() where T : Component
        {
            return (from Component comp in components where comp is T select comp as T).ToArray();
        }

        public T[] GetComponents<T>(Predicate<T> filter) where T : Component
        {
            return (from Component comp in components where comp is T && filter(comp as T) select comp as T).ToArray();
        }

        public Component[] Getcomponents(Predicate<Component> filter)
        {
            return components.FindAll(filter).ToArray();
        }

        public void Update()
        {
            IUpdatable[] Updateables = (from Component component in components where component is IUpdatable select component as IUpdatable).ToArray();

            foreach (IUpdatable Updateable in Updateables)
                Updateable.Update();
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            IDrawable[] IDrawables = (from Component component in components where component is IDrawable select component as IDrawable).ToArray();

            foreach (IDrawable IDrawable in IDrawables)
                IDrawable.Draw(spriteBatch);
        }

        public void LoadContent(ContentManager content)
        {
            ILoadable[] ILoadables = (from Component component in components where component is ILoadable select component as ILoadable).ToArray();

            foreach (ILoadable ILoadable in ILoadables)
                ILoadable.LoadContent(content);
        }
    }
}
